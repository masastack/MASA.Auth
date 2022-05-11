// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyIdpDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public AuthenticationTypes AuthenticationType { get; set; }

    public AddThirdPartyIdpDto(string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        AuthenticationType = authenticationType;
    }

    public static implicit operator AddThirdPartyIdpDto(ThirdPartyIdpDetailDto platform)
    {
        return new AddThirdPartyIdpDto(platform.Name, platform.DisplayName, platform.ClientId, platform.ClientSecret, platform.Url, platform.Icon, platform.AuthenticationType);
    }
}
