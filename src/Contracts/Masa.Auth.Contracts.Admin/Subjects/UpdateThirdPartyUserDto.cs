// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateThirdPartyUserDto
{
    public Guid Id { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; } = "";

    public string ExtendedData { get; private set; } = "";

    public UpdateThirdPartyUserDto(Guid id, bool enabled, string thridPartyIdentity, string extendedData)
    {
        Id = id;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        ExtendedData = extendedData;
    }
}
