// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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

    public Dictionary<string, string> JsonKeyMap = new();

    public bool MapAll { get; set; }

    public void BindOAuthOptions(OAuthOptions options)
    {
        options.SignInScheme = AuthenticationExternalConstants.ExternalCookieAuthenticationScheme;
        options.ClaimsIssuer = Scheme;
        options.CallbackPath = CallbackPath;
        options.AuthorizationEndpoint = AuthorizationEndpoint;
        options.TokenEndpoint = TokenEndpoint;
        options.UserInformationEndpoint = UserInformationEndpoint;
        options.ClientId = "";
        options.ClientSecret = "";
        if (MapAll) options.ClaimActions.MapAll();
        foreach(var (key,value) in JsonKeyMap)
        {
            options.ClaimActions.MapJsonKey(key, value);
        }
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
