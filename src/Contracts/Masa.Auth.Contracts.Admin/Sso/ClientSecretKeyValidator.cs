// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Masa.Auth.Contracts.Admin.Sso;

/// <summary>
/// 验签公钥类 secret（JWK / X509CertificateBase64）保存前校验；须拦截私钥、弱钥与坏格式，失败原因不含 secret 内容。
/// 原样入库供 SSO 解析为 SecurityKey，否则易出现私钥泄露或验签静默失败。
/// </summary>
public static class ClientSecretKeyValidator
{
    public const string JwkType = "JWK";
    public const string X509CertificateBase64Type = "X509CertificateBase64";

    /// <summary>RFC 7518 §6：私钥与对称密钥参数。出现任何一个都意味着这不是一把"公钥"。</summary>
    static readonly string[] PrivateJwkMembers = { "d", "p", "q", "dp", "dq", "qi", "oth", "k" };

    static readonly string[] AllowedCurves = { "P-256", "P-384", "P-521" };

    /// <summary>RSA 模数最小位数；与 NIST SP 800-131A 对 RSA 签名的最低要求一致。</summary>
    public const int MinRsaKeyBits = 2048;

    /// <summary>非验签公钥类型（如 SharedSecret）直接放行。</summary>
    /// <returns>通过返回 <c>null</c>；否则返回失败原因（英文短句，不含 secret 内容）</returns>
    public static string? Validate(string? secretType, string? value)
    {
        if (string.Equals(secretType, JwkType, StringComparison.OrdinalIgnoreCase))
        {
            return ValidateJwk(value);
        }

        if (string.Equals(secretType, X509CertificateBase64Type, StringComparison.OrdinalIgnoreCase))
        {
            return ValidateCertificate(value);
        }

        return null;
    }

    static string? ValidateJwk(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "JWK is empty";
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(value);
        }
        catch (JsonException)
        {
            return "JWK is not valid JSON";
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return "JWK must be a JSON object";
            }

            // 仅接受单 JWK；JWKS 在 SSO 侧无法按单钥解析
            if (root.TryGetProperty("keys", out _))
            {
                return "a JWK Set is not accepted, register one JWK per secret";
            }

            // 优先检出私钥参数（安全类错误）
            foreach (var member in PrivateJwkMembers)
            {
                if (root.TryGetProperty(member, out var memberValue) && CarriesKeyMaterial(memberValue))
                {
                    return $"JWK contains private key parameter '{member}', register the public key only";
                }
            }

            var kty = GetString(root, "kty");
            if (string.IsNullOrEmpty(kty))
            {
                return "JWK has no kty";
            }

            if (string.IsNullOrWhiteSpace(GetString(root, "kid")))
            {
                // 多钥轮换须带 kid 以便精确匹配
                return "JWK has no kid";
            }

            var use = GetString(root, "use");
            if (use is not null && !string.Equals(use, "sig", StringComparison.Ordinal))
            {
                return "JWK 'use' must be 'sig' when present";
            }

            switch (kty)
            {
                case "RSA":
                    return ValidateRsaJwk(root);
                case "EC":
                    return ValidateEcJwk(root);
                default:
                    // oct 为对称密钥；其它 kty SSO 不接受
                    return $"JWK kty '{kty}' is not accepted, use RSA or EC";
            }
        }
    }

    static string? ValidateRsaJwk(JsonElement root)
    {
        var n = GetString(root, "n");
        var e = GetString(root, "e");
        if (string.IsNullOrEmpty(n) || string.IsNullOrEmpty(e))
        {
            return "RSA JWK requires 'n' and 'e'";
        }

        if (!TryDecodeBase64Url(n, out var modulus))
        {
            return "RSA JWK 'n' is not valid base64url";
        }

        if (!TryDecodeBase64Url(e, out _))
        {
            return "RSA JWK 'e' is not valid base64url";
        }

        return BitLength(modulus) < MinRsaKeyBits
            ? $"RSA key is shorter than {MinRsaKeyBits} bits"
            : null;
    }

    static string? ValidateEcJwk(JsonElement root)
    {
        var crv = GetString(root, "crv");
        if (string.IsNullOrEmpty(crv) || !AllowedCurves.Contains(crv, StringComparer.Ordinal))
        {
            return "EC JWK 'crv' must be one of P-256, P-384, P-521";
        }

        var x = GetString(root, "x");
        var y = GetString(root, "y");
        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
        {
            return "EC JWK requires 'x' and 'y'";
        }

        if (!TryDecodeBase64Url(x, out _) || !TryDecodeBase64Url(y, out _))
        {
            return "EC JWK 'x'/'y' is not valid base64url";
        }

        return null;
    }

    static string? ValidateCertificate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "certificate is empty";
        }

        byte[] raw;
        try
        {
            raw = Convert.FromBase64String(value.Trim());
        }
        catch (FormatException)
        {
            return "certificate is not valid base64";
        }

        X509Certificate2 certificate;
        try
        {
            // 仅 DER 公钥证书；PFX 无法载入，从结构上拒绝带私钥包
            certificate = X509CertificateLoader.LoadCertificate(raw);
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            return "certificate must be the base64 of a DER-encoded public certificate "
                   + "(PEM headers and PKCS#12/PFX bundles are not accepted)";
        }

        using (certificate)
        {
            using var rsa = certificate.GetRSAPublicKey();
            if (rsa is not null)
            {
                return rsa.KeySize < MinRsaKeyBits
                    ? $"RSA key is shorter than {MinRsaKeyBits} bits"
                    : null;
            }

            using var ecdsa = certificate.GetECDsaPublicKey();
            return ecdsa is null
                ? "certificate public key must be RSA or ECDSA"
                : null;
        }
    }

    /// <summary>空集合（如 <c>"oth":[]</c>）不算私钥材料；有任何实质内容则按私钥拒绝。</summary>
    static bool CarriesKeyMaterial(JsonElement value)
        => value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => false,
            JsonValueKind.String => !string.IsNullOrEmpty(value.GetString()),
            JsonValueKind.Array => value.GetArrayLength() > 0,
            JsonValueKind.Object => value.EnumerateObject().Any(),
            _ => true
        };

    static string? GetString(JsonElement element, string name)
        => element.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    static bool TryDecodeBase64Url(string value, out byte[] bytes)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
            case 1:
                bytes = Array.Empty<byte>();
                return false;
        }

        try
        {
            bytes = Convert.FromBase64String(padded);
            return true;
        }
        catch (FormatException)
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    /// <summary>大端无符号整数的有效位数（忽略前导 0 字节）。</summary>
    static int BitLength(byte[] bigEndian)
    {
        var index = 0;
        while (index < bigEndian.Length && bigEndian[index] == 0)
        {
            index++;
        }

        if (index == bigEndian.Length)
        {
            return 0;
        }

        var bits = (bigEndian.Length - index - 1) * 8;
        var top = bigEndian[index];
        while (top != 0)
        {
            bits++;
            top >>= 1;
        }

        return bits;
    }
}
