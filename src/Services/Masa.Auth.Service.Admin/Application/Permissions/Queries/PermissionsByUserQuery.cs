// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionsByUserQuery(Guid User, List<Guid>? Teams = null) : Query<List<Guid>>
{
    public List<KeyValuePair<Guid, bool>> UserPermissionIds { get; set; } = new();

    public List<Guid> RolePermissionIds { get; set; } = new();

    public List<Guid> TeamPermissionIds { get; set; } = new();

    public override List<Guid> Result { get; set; } = new();
}
