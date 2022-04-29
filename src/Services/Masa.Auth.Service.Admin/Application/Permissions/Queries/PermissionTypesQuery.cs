// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionTypesQuery : Query<List<SelectItemDto<int>>>
{
    public override List<SelectItemDto<int>> Result { get; set; } = new();
}
