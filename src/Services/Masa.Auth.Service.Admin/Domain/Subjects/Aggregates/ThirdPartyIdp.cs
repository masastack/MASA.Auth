// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyIdp : IdentityProvider
{
    public string ClientId { get; private set; }

    public string ClientSecret { get; private set; }

    public string Url { get; private set; }

    public string VerifyFile { get; private set; }

    public AuthenticationTypes VerifyType { get; private set; }

    public ThirdPartyIdp(string name, string displayName, string icon, bool enabled, IdentificationTypes identificationType, string clientId, string clientSecret, string url, string verifyFile, AuthenticationTypes verifyType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        Enabled = enabled;
        VerifyFile = verifyFile;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }

    public void Update(string displayName, string icon, bool enabled, IdentificationTypes identificationType, string clientId, string clientSecret, string url, string verifyFile, AuthenticationTypes verifyType)
    {
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        Enabled = enabled;
        VerifyFile = verifyFile;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }

    public static implicit operator ThirdPartyIdpDetailDto(ThirdPartyIdp tpIdp)
    {
        return new ThirdPartyIdpDetailDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.ClientSecret, tpIdp.Url, tpIdp.Icon, tpIdp.VerifyFile, tpIdp.Enabled, tpIdp.VerifyType, tpIdp.IdentificationType, tpIdp.CreationTime, tpIdp.ModificationTime);
    }
}

