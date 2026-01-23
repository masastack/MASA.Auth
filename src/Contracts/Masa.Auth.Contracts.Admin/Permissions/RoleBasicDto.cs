// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleBasicDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public int Limit { get; set; }

    public RoleTypes Type { get; set; }

    public string? Description { get; set; }

    public bool Enabled { get; set; }

    public RoleBasicDto(Guid id, string name, string code, int limit, RoleTypes type, string? description, bool enabled)
    {
        Id = id;
        Name = name;
        Code = code;
        Limit = limit;
        Type = type;
        Description = description;
        Enabled = enabled;
    }
}
