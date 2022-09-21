// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Text.Json.Serialization;

namespace Masa.Auth.Security.OAuth.Providers;

public class AuthenticationDefaults
{
    public string Scheme { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string CallbackPath { get; set; } = "";

    public string Issuer { get; set; } = "";

    public string AuthorizationEndpoint { get; set; } = "";

    public string TokenEndpoint { get; set; } = "";

    public string UserInformationEndpoint { get; set; } = "";

    public string Icon { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public Dictionary<string, string> JsonKeyMap = new();

    public bool MapAll { get; set; }

    [JsonIgnore]
    public Type HandlerType { get; set; } = typeof(OAuthHandler<OAuthOptions>);

    public void BindOAuthOptions(OAuthOptions options)
    {
        options.SignInScheme = AuthenticationExternalConstants.ExternalCookieAuthenticationScheme;
        options.ClaimsIssuer = Scheme;
        options.CallbackPath = CallbackPath;
        options.AuthorizationEndpoint = AuthorizationEndpoint;
        options.TokenEndpoint = TokenEndpoint;
        options.UserInformationEndpoint = UserInformationEndpoint;
        options.ClientId = ClientId;
        options.ClientSecret = ClientSecret;
        options.ClaimActions.Clear();
        if (MapAll) options.ClaimActions.MapAll();
        foreach (var (key,value) in JsonKeyMap)
        {
            options.ClaimActions.MapJsonKey(key, value);
        }
    }

    public static implicit operator AuthenticationScheme(AuthenticationDefaults authenticationDefaults)
    {
        return new AuthenticationScheme(authenticationDefaults.Scheme, authenticationDefaults.DisplayName, authenticationDefaults.HandlerType);
    }

    public override bool Equals(object? obj)
    {
        return obj is AuthenticationDefaults value && value.Scheme == Scheme;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}
