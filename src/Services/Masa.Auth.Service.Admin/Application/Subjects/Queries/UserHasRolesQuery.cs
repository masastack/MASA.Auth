// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserHasRolesQuery(Guid UserId, List<Guid> RoleIds) : Query<List<Guid>>
{
    public override List<Guid> Result { get; set; } = new();
}
