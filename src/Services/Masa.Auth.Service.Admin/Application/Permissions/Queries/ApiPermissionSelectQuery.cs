// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ApiPermissionSelectQuery(string Name) : Query<List<SelectItemDto<Guid>>>
{
    public int MaxCount { get; set; } = 20;

    public override List<SelectItemDto<Guid>> Result { get; set; } = new();
}
