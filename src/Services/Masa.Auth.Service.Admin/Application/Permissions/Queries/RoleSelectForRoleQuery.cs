﻿namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleSelectForRoleQuery(Guid RoleId) : Query<List<RoleSelectDto>>
{
    public override List<RoleSelectDto> Result { get; set; } = new();
}
