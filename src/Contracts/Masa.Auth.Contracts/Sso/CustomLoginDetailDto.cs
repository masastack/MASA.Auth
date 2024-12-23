// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginDetailDto : CustomLoginDto
{
    public List<CustomLoginThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    public CustomLoginDetailDto() { }

    public CustomLoginDetailDto(int id, string name, string title, ClientDto client, bool enabled, DateTime creationTime, DateTime? modificationTime, string creator, string modifier, List<CustomLoginThirdPartyIdpDto> thirdPartyIdps, List<RegisterFieldDto> registerFields) : base(id, name, title, client, enabled, creationTime, modificationTime, creator, modifier)
    {
        ThirdPartyIdps = thirdPartyIdps;
        RegisterFields = registerFields;
    }

    public static implicit operator UpdateCustomLoginDto(CustomLoginDetailDto customLogin)
    {
        return new UpdateCustomLoginDto(customLogin.Id, customLogin.Name, customLogin.Title, customLogin.Enabled, customLogin.ThirdPartyIdps, customLogin.RegisterFields);
    }
}

