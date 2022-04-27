using Masa.Auth.Contracts.Admin.Sso;

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class ClientConsts
{
    public static List<string> GetSecretTypes()
    {
        var secretTypes = new List<string>
            {
                "SharedSecret",
                "X509Thumbprint",
                "X509Name",
                "X509CertificateBase64",
                "JWK"
            };

        return secretTypes;
    }

    public static List<string> GetStandardClaims()
    {
        //http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
        var standardClaims = new List<string>
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
            };

        return standardClaims;
    }

    public static List<string> SigningAlgorithms()
    {
        var signingAlgorithms = new List<string>
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
            };

        return signingAlgorithms;
    }

    public static List<SelectItemDto<string>> GetProtocolTypes()
    {
        var protocolTypes = new List<SelectItemDto<string>> { new SelectItemDto<string>("oidc", "OpenID Connect") };

        return protocolTypes;
    }

    public static List<SelectItemDto<int>> GetAccessTokenTypes()
    {
        var accessTokenTypes = Enum<AccessTokenType>.GetEnumObjectDictionary().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Value,
            Value = pt.Key
        }).ToList();
        return accessTokenTypes;
    }

    public static List<SelectItemDto<int>> GetTokenExpirations()
    {
        var tokenExpirations = Enum<TokenExpiration>.GetEnumObjectDictionary().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Value,
            Value = pt.Key
        }).ToList();
        return tokenExpirations;
    }

    public static List<SelectItemDto<int>> GetTokenUsage()
    {
        var tokenUsage = Enum<TokenUsage>.GetEnumObjectDictionary().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Value,
            Value = pt.Key
        }).ToList();
        return tokenUsage;
    }
}
