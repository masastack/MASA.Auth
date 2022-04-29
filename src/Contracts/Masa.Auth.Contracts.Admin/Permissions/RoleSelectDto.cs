// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Limit { get; set; }

    public int AvailableQuantity { get; set; }

    public RoleSelectDto(Guid id, string name, int limit, int availableQuantity)
    {
        Id = id;
        Name = name;
        Limit = limit;
        AvailableQuantity = availableQuantity;
    }
}
