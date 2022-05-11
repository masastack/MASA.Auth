// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyUserDetailDto : ThirdPartyUserDto
{
    public new ThirdPartyIdpDetailDto ThirdPartyIdp { get; set; } = new();

    public new UserDetailDto User { get; set; } = new();

    public ThirdPartyUserDetailDto()
    {

    }

    public ThirdPartyUserDetailDto(Guid id, bool enabled, ThirdPartyIdpDetailDto thirdPartyIdp, UserDetailDto user, DateTime creationTime, DateTime? modificationTime, string creator, string modifier) : base(id, enabled, thirdPartyIdp, user, creationTime, modificationTime, creator, modifier)
    {
        ThirdPartyIdp = thirdPartyIdp;
        User = user;
    }
}


