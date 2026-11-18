// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>RFC 7523 断言校验：算法、iss/aud、生命周期上限、jti 等"签名有效 ≠ 可用"的前置条件。</summary>
public class DomainAssertionValidatorTests
{
    const string ClientId = "domainidentity";
    const string Audience = "https://sso.example.com/connect/token";

    readonly RSA _rsa = RSA.Create(2048);
    readonly IReadOnlyList<SecurityKey> _keys;

    public DomainAssertionValidatorTests()
    {
        var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(_rsa) { KeyId = "k1" });
        _keys = new[] { new JsonWebKey(JsonSerializer.Serialize(jwk)) };
    }

    [Fact]
    public void Valid_assertion_is_accepted_and_exposes_subject_and_jti()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, jti: "jti-1", sub: "mayue");

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.True(result.IsValid);
        Assert.Equal("mayue", result.Principal!.FindFirst("sub")!.Value);
        Assert.Equal("jti-1", result.Jti);
        Assert.NotNull(result.ExpiresUtc);
    }

    [Fact]
    public void Missing_jti_is_rejected_because_replay_guard_has_no_anchor()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, jti: null);

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
        Assert.Contains("jti", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Missing_iat_is_rejected_because_it_anchors_both_the_lifetime_cap_and_the_future_check()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, omitIat: true);

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
        Assert.Contains("iat", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NotBefore_slightly_in_the_future_is_tolerated_as_clock_drift()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(
            issuedAt: now.AddSeconds(30),          // 签发方时钟比 SSO 快 30 秒
            expires: now.AddSeconds(90),
            notBefore: now.AddSeconds(30));

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.True(result.IsValid, result.Error);
    }

    [Fact]
    public void Algorithm_none_is_rejected_before_signature_check()
    {
        var unsigned = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = Audience,
            Subject = new ClaimsIdentity(new[] { new Claim("sub", "mayue") }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            IssuedAt = DateTime.UtcNow,
            Claims = new Dictionary<string, object> { ["jti"] = "jti-none" },
            SigningCredentials = null
        });

        var result = DomainAssertionValidator.Validate(unsigned, ClientId, _keys, new[] { Audience }, DateTime.UtcNow);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Symmetric_hs256_is_rejected_even_when_client_key_material_could_validate_it()
    {
        // alg 混淆：用公开公钥材料当 HS 密钥
        var key = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));
        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = Audience,
            Subject = new ClaimsIdentity(new[] { new Claim("sub", "mayue") }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            IssuedAt = DateTime.UtcNow,
            Claims = new Dictionary<string, object> { ["jti"] = "jti-hs" },
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        });

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, DateTime.UtcNow);

        Assert.False(result.IsValid);
        Assert.Contains("symmetric", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Audience_must_match_this_token_endpoint()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, audience: "https://other-sso.example.com/connect/token");

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Issuer_must_equal_the_authenticated_client_id()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, issuer: "some-other-client");

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Expired_assertion_is_rejected()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now.AddMinutes(-10), expires: now.AddMinutes(-5));

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Lifetime_beyond_the_cap_is_rejected()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now, expires: now.AddHours(1));

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
        Assert.Contains("lifetime", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validation_uses_the_injected_clock_not_the_system_clock()
    {
        var issuedAt = DateTime.UtcNow;
        var token = CreateToken(issuedAt);

        Assert.True(DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, issuedAt).IsValid);
        Assert.False(DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, issuedAt.AddHours(1)).IsValid);
    }

    [Fact]
    public void Lifetime_exactly_at_the_cap_is_accepted_and_one_second_beyond_is_rejected()
    {
        var now = DateTime.UtcNow;
        var cap = DomainAssertionValidator.MaxLifetimeSeconds;

        var atCap = CreateToken(now, expires: now.AddSeconds(cap));
        var beyondCap = CreateToken(now, expires: now.AddSeconds(cap + 1));

        Assert.True(DomainAssertionValidator.Validate(atCap, ClientId, _keys, new[] { Audience }, now).IsValid);
        Assert.False(DomainAssertionValidator.Validate(beyondCap, ClientId, _keys, new[] { Audience }, now).IsValid);
    }

    [Fact]
    public void Expiry_within_the_clock_skew_is_tolerated_and_beyond_it_is_rejected()
    {
        var now = DateTime.UtcNow;
        var skew = DomainAssertionValidator.ClockSkewSeconds;

        var expiredWithinSkew = CreateToken(now.AddMinutes(-2), expires: now.AddSeconds(-(skew - 10)));
        var expiredBeyondSkew = CreateToken(now.AddMinutes(-2), expires: now.AddSeconds(-(skew + 10)));

        Assert.True(DomainAssertionValidator.Validate(expiredWithinSkew, ClientId, _keys, new[] { Audience }, now).IsValid);
        Assert.False(DomainAssertionValidator.Validate(expiredBeyondSkew, ClientId, _keys, new[] { Audience }, now).IsValid);
    }

    [Fact]
    public void Asymmetric_ecdsa_assertion_is_accepted()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(new ECDsaSecurityKey(ecdsa) { KeyId = "ec1" });
        var keys = new SecurityKey[] { new JsonWebKey(JsonSerializer.Serialize(jwk)) };
        var now = DateTime.UtcNow;

        var token = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = Audience,
            Subject = new ClaimsIdentity(new[] { new Claim("sub", "mayue") }),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(2),
            Claims = new Dictionary<string, object> { ["jti"] = "jti-ec" },
            SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(ecdsa) { KeyId = "ec1" }, SecurityAlgorithms.EcdsaSha256)
        });

        var result = DomainAssertionValidator.Validate(token, ClientId, keys, new[] { Audience }, now);

        Assert.True(result.IsValid, result.Error);
    }

    [Fact]
    public void Issued_in_the_future_beyond_skew_is_rejected()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now.AddMinutes(10), expires: now.AddMinutes(11));

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Assertion_signed_by_a_different_key_is_rejected()
    {
        var now = DateTime.UtcNow;
        using var otherRsa = RSA.Create(2048);
        var token = CreateToken(now, rsa: otherRsa);

        var result = DomainAssertionValidator.Validate(token, ClientId, _keys, new[] { Audience }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Client_without_registered_JWK_key_is_rejected()
    {
        var now = DateTime.UtcNow;
        var token = CreateToken(now);

        var result = DomainAssertionValidator.Validate(token, ClientId, Array.Empty<SecurityKey>(), new[] { Audience }, now);

        Assert.False(result.IsValid);
        Assert.Contains("JWK", result.Error);
    }

    [Fact]
    public void Malformed_assertion_is_rejected_without_throwing()
    {
        var result = DomainAssertionValidator.Validate("not-a-jwt", ClientId, _keys, new[] { Audience }, DateTime.UtcNow);

        Assert.False(result.IsValid);
    }

    string CreateToken(
        DateTime issuedAt,
        string? jti = "jti-default",
        string? sub = "mayue",
        string? issuer = ClientId,
        string? audience = Audience,
        DateTime? expires = null,
        DateTime? notBefore = null,
        RSA? rsa = null,
        bool omitIat = false)
    {
        var claims = new List<Claim>();
        if (sub is not null)
        {
            claims.Add(new Claim("sub", sub));
        }

        var expiresAt = expires ?? issuedAt.AddMinutes(2);
        var notBeforeAt = notBefore ?? issuedAt;

        if (omitIat)
        {
            // CreateToken 会自填 iat；无 iat 须 JwtPayload 直构
            var header = new JwtHeader(new SigningCredentials(
                new RsaSecurityKey(rsa ?? _rsa) { KeyId = "k1" },
                SecurityAlgorithms.RsaSha256));
            var payload = new JwtPayload(issuer, audience, claims, notBeforeAt, expiresAt);
            if (jti is not null)
            {
                payload.Add("jti", jti);
            }

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(header, payload));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            Expires = expiresAt,
            NotBefore = notBeforeAt,
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa ?? _rsa) { KeyId = "k1" },
                SecurityAlgorithms.RsaSha256)
        };

        if (jti is not null)
        {
            descriptor.Claims = new Dictionary<string, object> { ["jti"] = jti };
        }

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    static class JsonSerializer
    {
        internal static string Serialize(object value) => System.Text.Json.JsonSerializer.Serialize(value);
    }
}
