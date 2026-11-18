// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class ClientConsts
{
    public static readonly string[] MandatoryResource = new string[] { "openid", "profile" };

    public static List<SelectItemDto<string>> GetSecretTypes()
    {
        var secretTypes = (new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64",
                "JWK"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return secretTypes;
    }

    /// <summary>
    /// 按客户端类型返回管理界面 grant_type 复选项过滤集合，不影响已登记的 AllowedGrantTypes。
    /// Web/Spa/Native：交互式 code 家族及 implicit/hybrid/password；Machine：client_credentials、密码类及 JwtBearer/TokenExchange；Device：设备流；未知类型返回空。
    /// </summary>
    public static IReadOnlyCollection<string> GetApplicableGrantTypes(ClientTypes clientType) => clientType switch
    {
        ClientTypes.Machine => new[]
        {
            GrantType.CLIENT_CREDENTIALS,
            GrantType.LDAP,
            GrantType.RESOURCE_OWNER_PASSWORD,
            GrantType.JWT_BEARER,
            GrantType.TOKEN_EXCHANGE
        },
        ClientTypes.Device => new[]
        {
            GrantType.DEVICE_FLOW
        },
        ClientTypes.Web or ClientTypes.Spa or ClientTypes.Native => new[]
        {
            GrantType.IMPLICIT,
            GrantType.HYBRID,
            GrantType.AUTHORIZATION_CODE,
            GrantType.PHONE_CODE,
            GrantType.EMAIL_CODE,
            GrantType.LOCAL_PHONE,
            GrantType.THIRD_PARTY_IDP,
            GrantType.LDAP,
            GrantType.RESOURCE_OWNER_PASSWORD
        },
        _ => Array.Empty<string>()
    };

    /// <summary>非对称验签公钥须原样保存；哈希后 IS4 无法解析为 SecurityKey，仅 SharedSecret 应哈希。</summary>
    public static bool IsVerificationKeySecret(string? secretType)
        => secretType is not null
           && (string.Equals(secretType, "JWK", StringComparison.OrdinalIgnoreCase)
               || string.Equals(secretType, "X509CertificateBase64", StringComparison.OrdinalIgnoreCase)
               || string.Equals(secretType, "X509Name", StringComparison.OrdinalIgnoreCase)
               || string.Equals(secretType, "X509Thumbprint", StringComparison.OrdinalIgnoreCase));

    public static List<SelectItemDto<string>> GetStandardClaims()
    {
        //http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        var standardClaims = (new List<string>
            {
                "name",
                "given_name",
                "family_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "address",
                "updated_at"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return standardClaims;
    }

    public static List<SelectItemDto<string>> SigningAlgorithms()
    {
        var signingAlgorithms = (new List<string>
            {
                "RS256",
                "RS384",
                "RS512",
                "PS256",
                "PS384",
                "PS512",
                "ES256",
                "ES384",
                "ES512"
            }).Select(a => new SelectItemDto<string> { Value = a, Text = a }).ToList();

        return signingAlgorithms;
    }

    public static List<SelectItemDto<string>> GetProtocolTypes()
    {
        var protocolTypes = new List<SelectItemDto<string>> { new SelectItemDto<string>("oidc", "OpenID Connect") };

        return protocolTypes;
    }

    public static List<SelectItemDto<int>> GetAccessTokenTypes()
    {
        var accessTokenTypes = EnumUtil.GetList<AccessTokenType>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return accessTokenTypes;
    }

    public static List<SelectItemDto<int>> GetTokenExpirations()
    {
        var tokenExpirations = EnumUtil.GetList<TokenExpiration>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return tokenExpirations;
    }

    public static List<SelectItemDto<int>> GetTokenUsage()
    {
        var tokenUsage = EnumUtil.GetList<TokenUsage>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
        return tokenUsage;
    }

    public const string AUTH_CLIENT_DEFAULT_URI = "https://cdn.masastack.com/stack/auth/ico/auth-client-default.svg";
}
