﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserHasAnyRoleQuery(Guid UserId, List<Guid> RoleIds) : Query<bool>
{
    public override bool Result { get; set; }
}
