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

    public AuthenticationDefaults()
    {

    }

    public AuthenticationDefaults(string scheme, string displayName, string callbackPath, string issuer, string authorizationEndpoint, string tokenEndpoint, string userInformationEndpoint)
    {
        Scheme = scheme;
        DisplayName = displayName;
        CallbackPath = callbackPath;
        Issuer = issuer;
        AuthorizationEndpoint = authorizationEndpoint;
        TokenEndpoint = tokenEndpoint;
        UserInformationEndpoint = userInformationEndpoint;
    }
}
