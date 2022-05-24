// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddThirdPartyUserDto
{
    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; } = "";

    public AddUserDto User { get; set; } = new();

    public AddThirdPartyUserDto(Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, AddUserDto user)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
    }
}
