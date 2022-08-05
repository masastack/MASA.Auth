// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamRoleSelectQuery(string Name, Guid UserId) : Query<List<TeamRoleSelectDto>>
{
    public override List<TeamRoleSelectDto> Result { get; set; } = new();
}
