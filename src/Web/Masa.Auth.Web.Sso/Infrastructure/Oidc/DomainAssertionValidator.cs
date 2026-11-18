// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using Masa.Utils.Ldap.Novell.Entries;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonWebKey = Microsoft.IdentityModel.Tokens.JsonWebKey;
using SecurityKey = Microsoft.IdentityModel.Tokens.SecurityKey;
using Secret = IdentityServer4.Models.Secret;

namespace Masa.Auth.Web.Sso.Infrastructure.Oidc;

/// <summary>
/// RFC 7523 §3 JWT Bearer 断言的解析与校验（IdentityModel 验签 + 算法白名单、iat/jti 必需、有效期上限）。
/// iss=client_id、sub=域账号；拒 none/HS*（公钥不能当 HMAC 密钥）、aud 绑定 token endpoint、exp-iat 上限、已过期 Secret 不参与验签。
/// </summary>
public static class DomainAssertionValidator
{
    /// <summary>断言允许的最长有效期（exp - iat，秒）；超过即拒绝，用来压缩重放窗口。</summary>
    public const int MaxLifetimeSeconds = 300;

    /// <summary>时钟偏移（秒）；jti 缓存 TTL 须覆盖 exp+skew，否则偏移窗口内可重放。</summary>
    public const int ClockSkewSeconds = 60;

    /// <summary>失败原因仅用于日志；对外统一 invalid_grant，避免成为探测 oracle。</summary>
    public sealed record Result(ClaimsPrincipal? Principal, string? Error, string? Jti, DateTime? ExpiresUtc)
    {
        public bool IsValid => Principal is not null;

        public static Result Fail(string error) => new(null, error, null, null);
    }

    /// <summary>从 JWK / X509CertificateBase64 构造验签公钥；Thumbprint/Name 无公钥不参与；Expiration 已过则跳过。</summary>
    public static IReadOnlyList<SecurityKey> BuildSigningKeys(IEnumerable<Secret>? clientSecrets, DateTime nowUtc)
    {
        var keys = new List<SecurityKey>();
        if (clientSecrets is null)
        {
            return keys;
        }

        foreach (var secret in clientSecrets.Where(s =>
                     string.Equals(s.Type, IdentityServerConstants.SecretTypes.JsonWebKey, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(s.Type, IdentityServerConstants.SecretTypes.X509CertificateBase64, StringComparison.OrdinalIgnoreCase)))
        {
            if (string.IsNullOrWhiteSpace(secret.Value))
            {
                continue;
            }

            // Secret.Expiration 表示该密钥失效时刻
            if (secret.Expiration.HasValue && secret.Expiration.Value < nowUtc)
            {
                continue;
            }

            try
            {
                if (string.Equals(secret.Type, IdentityServerConstants.SecretTypes.X509CertificateBase64, StringComparison.OrdinalIgnoreCase))
                {
                    // X509SecurityKey 须保持存活至验签结束，不可 Dispose
                    keys.Add(new X509SecurityKey(X509CertificateLoader.LoadCertificate(Convert.FromBase64String(secret.Value.Trim()))));
                }
                else
                {
                    keys.Add(new JsonWebKey(secret.Value));
                }
            }
            catch (Exception)
            {
                // 单个坏密钥不阻断：其余密钥仍可验签
            }
        }

        return keys;
    }

    /// <summary>
    /// 由 SSO 对外地址推出断言可接受的 <c>aud</c>：每个地址归一（去空白与末尾 <c>/</c>）后产出 <c>{issuer}</c> 与 <c>{issuer}/connect/token</c>。
    /// 反代后 <c>Request.Host</c> 常为内网地址，故应传配置的对外域名；空白项忽略，全部为空时结果为空。比较为 Ordinal。
    /// </summary>
    public static IReadOnlyCollection<string> BuildAudiences(params string?[] issuers)
    {
        var audiences = new HashSet<string>(StringComparer.Ordinal);
        foreach (var issuer in issuers)
        {
            var normalized = issuer?.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(normalized))
            {
                continue;
            }

            audiences.Add(normalized);
            audiences.Add($"{normalized}/connect/token");
        }

        return audiences;
    }

    /// <summary>校验 assertion；<paramref name="now"/> 注入后 exp/nbf/iat 判定不读系统时钟。</summary>
    public static Result Validate(
        string? assertion,
        string clientId,
        IReadOnlyList<SecurityKey> signingKeys,
        IReadOnlyCollection<string> expectedAudience,
        DateTime now)
    {
        var skew = TimeSpan.FromSeconds(ClockSkewSeconds);

        if (string.IsNullOrWhiteSpace(assertion))
        {
            return Result.Fail("assertion is missing");
        }

        if (signingKeys.Count == 0)
        {
            return Result.Fail("client has no registered JWK signing key");
        }

        JsonWebToken token;
        try
        {
            token = new JsonWebTokenHandler().ReadJsonWebToken(assertion);
        }
        catch (Exception)
        {
            return Result.Fail("assertion is not a well-formed JWT");
        }

        // 先于验签拒 none/HS，给出明确失败原因
        if (string.IsNullOrEmpty(token.Alg) || token.Alg.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("assertion must be signed (alg=none rejected)");
        }

        if (token.Alg.StartsWith("HS", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("symmetric algorithms are not accepted for assertions");
        }

        var validation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = clientId,
            ValidateAudience = true,
            ValidAudiences = expectedAudience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidAlgorithms = AllowedAlgorithms,
            ClockSkew = skew,
            // 生命周期用注入的 now，与下方 iat/有效期上限判定一致
            LifetimeValidator = (notBefore, expires, _, _) =>
                expires is not null
                && expires.Value.Add(skew) > now
                && (notBefore is null || notBefore.Value <= now.Add(skew))
        };

        try
        {
            var handler = new JsonWebTokenHandler();
            var result = handler.ValidateTokenAsync(assertion, validation).GetAwaiter().GetResult();
            if (!result.IsValid || result.ClaimsIdentity is null)
            {
                return Result.Fail(result.Exception?.Message ?? "assertion signature or claims invalid");
            }

            var principal = new ClaimsPrincipal(result.ClaimsIdentity);

            var iat = token.IssuedAt == default ? (DateTime?)null : token.IssuedAt;
            var exp = token.ValidTo == default ? (DateTime?)null : token.ValidTo;

            if (exp is null)
            {
                return Result.Fail("assertion has no exp");
            }

            // iat 必需：无 iat 则无法约束 exp-iat 上限（jti 不限制首次兑换时点）
            if (iat is null)
            {
                return Result.Fail("assertion has no iat");
            }

            // exp-iat 超过上限则拒绝
            if ((exp.Value - iat.Value).TotalSeconds > MaxLifetimeSeconds)
            {
                return Result.Fail($"assertion lifetime exceeds {MaxLifetimeSeconds}s");
            }

            // iat 明显超前（超出 skew）则拒绝
            if (iat.Value > now.Add(skew))
            {
                return Result.Fail("assertion iat is in the future");
            }

            var jti = token.Id;
            if (string.IsNullOrWhiteSpace(jti))
            {
                // jti 缺失则无法做重放防护
                return Result.Fail("assertion has no jti");
            }

            return new Result(principal, null, jti, exp);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    // RFC 7518 §3 非对称 alg 白名单
    static readonly string[] AllowedAlgorithms =
    {
        SecurityAlgorithms.RsaSha256, SecurityAlgorithms.RsaSha384, SecurityAlgorithms.RsaSha512,
        SecurityAlgorithms.RsaSsaPssSha256, SecurityAlgorithms.RsaSsaPssSha384, SecurityAlgorithms.RsaSsaPssSha512,
        SecurityAlgorithms.EcdsaSha256, SecurityAlgorithms.EcdsaSha384, SecurityAlgorithms.EcdsaSha512
    };
}
