// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddCustomLoginDto
{
    public string Name { get; set; } = "";

    public string Title { get; set; } = "";

    public int ClientId { get; set; }

    public bool Enabled { get; set; } = true;

    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public AddCustomLoginDto()
    {

    }

    public AddCustomLoginDto(string name, string title, int clientId, bool enabled, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields)
    {
        Name = name;
        Title = title;
        ClientId = clientId;
        Enabled = enabled;
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }
}

