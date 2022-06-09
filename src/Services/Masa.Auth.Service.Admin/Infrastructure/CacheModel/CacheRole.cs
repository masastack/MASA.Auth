// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModel;

public class CacheRole
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public int Limit { get; set; }

    public List<Guid> Permissions { get; set; } = new();

    public List<Guid> ChildrenRoles { get; set; } = new();
}
