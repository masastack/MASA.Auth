// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDetailDto : ThirdPartyIdpDto
{
    public ThirdPartyIdpDetailDto() : base()
    {

    }

    public ThirdPartyIdpDetailDto(Guid id, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime) : base(id, name, displayName, clientId, clientSecret, url, icon, authenticationType, creationTime, modificationTime)
    {
    }
}

