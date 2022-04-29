// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ApplicationPermissionsQuery(string SystemId) : Query<List<AppPermissionDto>>
{
    public override List<AppPermissionDto> Result { get; set; } = new();
}
