// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpsertThirdPartyUserDto : AddThirdPartyUserDto
{
    public UpsertThirdPartyUserDto(Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, string extendedData, AddUserDto user) : base(thirdPartyIdpId, enabled, thridPartyIdentity, extendedData, user)
    {
    }
}
