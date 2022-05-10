// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddUserClaimDto
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public UserClaimType UserClaimType { get; set; }

    public AddUserClaimDto()
    {
    }

    public AddUserClaimDto(string name, string description, UserClaimType userClaimType)
    {
        Name = name;
        Description = description;
        UserClaimType = userClaimType;
    }
}

