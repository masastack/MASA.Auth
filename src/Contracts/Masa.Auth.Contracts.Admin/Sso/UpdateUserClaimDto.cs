// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateUserClaimDto : AddUserClaimDto
{
    public int Id { get; set; }

    public UpdateUserClaimDto()
    {
    }

    public UpdateUserClaimDto(int id, string name, string description, UserClaimType userClaimType) : base(name, description, userClaimType)
    {
        Id = id;
    }
}

