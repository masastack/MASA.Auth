// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record GetI18NDisplayNameQuery(string[] CultureName, string Name) : Query<List<PermissionI18NDisplayNameDto>>
{
    public override List<PermissionI18NDisplayNameDto> Result { get; set; } = new();
}
