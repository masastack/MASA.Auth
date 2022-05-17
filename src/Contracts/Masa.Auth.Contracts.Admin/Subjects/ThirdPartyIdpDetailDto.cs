// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDetailDto : ThirdPartyIdpDto
{
    public ThirdPartyIdpDetailDto()
    {
    }

    public ThirdPartyIdpDetailDto(Guid id, string name, string displayName, string clientId, string clientSecret, string url, string icon, string verifyFile, bool enabled, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime) : base(id, name, displayName, clientId, clientSecret, url, icon, verifyFile, enabled, authenticationType, creationTime, modificationTime)
    {
    }
}

