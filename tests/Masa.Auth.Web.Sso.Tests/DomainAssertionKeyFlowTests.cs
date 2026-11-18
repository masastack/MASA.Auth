// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>调用方签断言与 SSO 从 ClientSecrets 验签的端到端（kid/算法/导入方式）。</summary>
public class DomainAssertionKeyFlowTests
{
    const string ClientId = "domainidentity";
    const string Authority = "https://psso.example.com";

    [Fact]
    public void Assertion_signed_with_caller_private_key_validates_with_registered_public_jwk()
    {
        using var rsa = RSA.Create(2048);
        const string KeyId = "domainidentity-2026";

        var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
            new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = KeyId });

        // Secret.Type 须用初始化器设置，否则验签静默失败
        var registeredSecrets = new[]
        {
            new IdentityServer4.Models.Secret
            {
                Value = System.Text.Json.JsonSerializer.Serialize(publicJwk),
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.JsonWebKey
            }
        };

        var now = DateTime.UtcNow;
        var keys = DomainAssertionValidator.BuildSigningKeys(registeredSecrets, now);
        Assert.Single(keys);

        var assertion = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = $"{Authority}/connect/token",
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("sub", "mayue"),
                new Claim("amr", "windows_negotiate")
            }),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddSeconds(60),
            Claims = new Dictionary<string, object> { ["jti"] = Guid.NewGuid().ToString("N") },
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa) { KeyId = KeyId },
                SecurityAlgorithms.RsaSha256)
        });

        var result = DomainAssertionValidator.Validate(
            assertion, ClientId, keys, new[] { $"{Authority}/connect/token" }, now);

        Assert.True(result.IsValid, result.Error);
        Assert.Equal("mayue", result.Principal!.FindFirst("sub")!.Value);
    }

    [Fact]
    public void Assertion_signed_with_a_key_the_client_never_registered_is_rejected()
    {
        using var registeredKey = RSA.Create(2048);
        using var attackerKey = RSA.Create(2048);
        const string KeyId = "domainidentity-2026";

        var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
            new RsaSecurityKey(registeredKey.ExportParameters(false)) { KeyId = KeyId });

        var now = DateTime.UtcNow;
        var keys = DomainAssertionValidator.BuildSigningKeys(new[]
        {
            new IdentityServer4.Models.Secret
            {
                Value = System.Text.Json.JsonSerializer.Serialize(publicJwk),
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.JsonWebKey
            }
        }, now);

        var assertion = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = $"{Authority}/connect/token",
            Subject = new ClaimsIdentity(new[] { new Claim("sub", "mayue") }),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddSeconds(60),
            Claims = new Dictionary<string, object> { ["jti"] = Guid.NewGuid().ToString("N") },
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(attackerKey) { KeyId = KeyId },
                SecurityAlgorithms.RsaSha256)
        });

        var result = DomainAssertionValidator.Validate(
            assertion, ClientId, keys, new[] { $"{Authority}/connect/token" }, now);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Assertion_signed_with_the_private_key_of_a_registered_certificate_validates()
    {
        using var rsa = RSA.Create(2048);
        using var signingCertificate = ClientSecretKeyValidatorTests.SelfSigned(rsa);
        var registeredPublicCertificate = Convert.ToBase64String(signingCertificate.Export(X509ContentType.Cert));

        var now = DateTime.UtcNow;
        var keys = DomainAssertionValidator.BuildSigningKeys(new[]
        {
            new IdentityServer4.Models.Secret
            {
                Value = registeredPublicCertificate,
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateBase64
            },
            new IdentityServer4.Models.Secret
            {
                Value = signingCertificate.Thumbprint,
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateThumbprint
            }
        }, now);
        Assert.Single(keys);

        var assertion = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = ClientId,
            Audience = $"{Authority}/connect/token",
            Subject = new ClaimsIdentity(new[] { new Claim("sub", "mayue") }),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddSeconds(60),
            Claims = new Dictionary<string, object> { ["jti"] = Guid.NewGuid().ToString("N") },
            SigningCredentials = new SigningCredentials(
                new X509SecurityKey(signingCertificate),
                SecurityAlgorithms.RsaSha256)
        });

        var result = DomainAssertionValidator.Validate(
            assertion, ClientId, keys, new[] { $"{Authority}/connect/token" }, now);

        Assert.True(result.IsValid, result.Error);
    }

    [Fact]
    public void Malformed_certificate_secret_is_skipped_without_disabling_the_other_keys()
    {
        using var rsa = RSA.Create(2048);
        using var certificate = ClientSecretKeyValidatorTests.SelfSigned(rsa);

        var keys = DomainAssertionValidator.BuildSigningKeys(new[]
        {
            new IdentityServer4.Models.Secret
            {
                Value = "not-a-certificate",
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateBase64
            },
            new IdentityServer4.Models.Secret
            {
                Value = Convert.ToBase64String(certificate.Export(X509ContentType.Cert)),
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.X509CertificateBase64
            }
        }, DateTime.UtcNow);

        Assert.Single(keys);
    }

    [Fact]
    public void Shared_secret_only_client_has_no_usable_signing_key()
    {
        var keys = DomainAssertionValidator.BuildSigningKeys(new[]
        {
            new IdentityServer4.Models.Secret
            {
                Value = "hashed-value",
                Type = IdentityServer4.IdentityServerConstants.SecretTypes.SharedSecret
            }
        }, DateTime.UtcNow);

        Assert.Empty(keys);
    }

    [Fact]
    public void Expired_registered_key_is_not_used_but_a_rotated_in_key_still_is()
    {
        using var oldRsa = RSA.Create(2048);
        using var newRsa = RSA.Create(2048);
        var now = DateTime.UtcNow;

        static IdentityServer4.Models.Secret Secret(RSA rsa, string kid, DateTime? expiration) => new()
        {
            Value = System.Text.Json.JsonSerializer.Serialize(JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = kid })),
            Type = IdentityServer4.IdentityServerConstants.SecretTypes.JsonWebKey,
            Expiration = expiration
        };

        var keys = DomainAssertionValidator.BuildSigningKeys(new[]
        {
            Secret(oldRsa, "old", now.AddMinutes(-1)),   // 已轮换下线
            Secret(newRsa, "new", null),                 // 无过期
            Secret(newRsa, "new-with-future-expiry", now.AddDays(1))
        }, now);

        Assert.Equal(2, keys.Count);
        Assert.DoesNotContain(keys, k => k.KeyId == "old");
    }
}
