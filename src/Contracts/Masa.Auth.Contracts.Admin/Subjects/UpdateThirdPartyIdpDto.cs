// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public bool Enabled { get; set; } = true;

    public string VerifyFile { get; set; } = "";

    public AuthenticationTypes VerifyType { get; set; }

    public UpdateThirdPartyIdpDto()
    {

    }

    public UpdateThirdPartyIdpDto(Guid id, string displayName, string clientId, string clientSecret, string url, string icon, bool enabled, string verifyFile, AuthenticationTypes verifyType)
    {
        Id = id;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        Enabled = enabled;
        VerifyFile = verifyFile;
        VerifyType = verifyType;
    }

    public static implicit operator UpdateThirdPartyIdpDto(ThirdPartyIdpDetailDto tpIdp)
    {
        return new UpdateThirdPartyIdpDto(tpIdp.Id, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.ClientSecret, tpIdp.Url, tpIdp.Icon, tpIdp.Enabled, tpIdp.VerifyFile, tpIdp.VerifyType);
    }
}
