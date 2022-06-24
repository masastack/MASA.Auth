// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyIdpDto
{
    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string Url { get; set; } = "";

    public bool Enabled { get; set; } = true;

    public string Icon { get; set; } = "";

    public string VerifyFile { get; set; } = "";

    public AuthenticationTypes VerifyType { get; set; }

    public IdentificationTypes IdentificationType { get; set; } = IdentificationTypes.PhoneNumber;

    public AddThirdPartyIdpDto()
    {
    }

    public AddThirdPartyIdpDto(string name, string displayName, string clientId, string clientSecret, string url, bool enabled, string icon, string verifyFile, AuthenticationTypes verifyType, IdentificationTypes identificationType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Enabled = enabled;
        Icon = icon;
        VerifyFile = verifyFile;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }
}
