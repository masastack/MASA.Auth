// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyIdpDto
{
    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string CallbackPath { get; set; } = "";

    public string AuthorizationEndpoint { get; set; } = "";

    public string TokenEndpoint { get; set; } = "";

    public string UserInformationEndpoint { get; set; } = "";

    public string Icon { get; set; } = "";

    public bool Enabled { get; set; }

    public AuthenticationTypes AuthenticationType { get; set; } = AuthenticationTypes.OAuth;

    public ThirdPartyIdpTypes ThirdPartyIdpType { get; set; } = ThirdPartyIdpTypes.Customize;

    public AddThirdPartyIdpDto()
    {
    }

    public AddThirdPartyIdpDto(string name, string displayName, string clientId, string clientSecret, string callbackPath, string authorizationEndpoint, string tokenEndpoint, string userInformationEndpoint, string icon, bool enabled, AuthenticationTypes authenticationType, ThirdPartyIdpTypes thirdPartyIdpType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        CallbackPath = callbackPath;
        AuthorizationEndpoint = authorizationEndpoint;
        TokenEndpoint = tokenEndpoint;
        UserInformationEndpoint = userInformationEndpoint;
        Icon = icon;
        Enabled = enabled;
        AuthenticationType = authenticationType;
        ThirdPartyIdpType = thirdPartyIdpType;
    }
}
