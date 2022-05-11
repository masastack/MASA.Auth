// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyIdp : IdentityProvider
{
    public string ClientId { get; private set; }

    public string ClientSecret { get; private set; }

    public string Url { get; private set; }

    public AuthenticationTypes VerifyType { get; private set; }

    public ThirdPartyIdp(string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes verifyType, IdentificationTypes identificationType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }

    public static implicit operator ThirdPartyIdpDetailDto(ThirdPartyIdp tpIdp)
    {
        return new ThirdPartyIdpDetailDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.ClientSecret, tpIdp.Url, tpIdp.Icon, tpIdp.VerifyType, tpIdp.CreationTime, tpIdp.ModificationTime);
    }
}

