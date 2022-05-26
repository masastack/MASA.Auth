// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyUserDto
{
    public Guid Id { get; set; }

    public bool Enabled { get; set; }

    public IdentityProviderDetailDto IdpDetailDto { get; set; } = new();

    public virtual UserDto User { get; set; } = new();

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; } = "";

    public string Modifier { get; set; } = "";

    public ThirdPartyUserDto()
    {

    }

    public ThirdPartyUserDto(Guid id, bool enabled, IdentityProviderDetailDto idpDetailDto, UserDto user, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        Id = id;
        Enabled = enabled;
        IdpDetailDto = idpDetailDto;
        User = user;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


