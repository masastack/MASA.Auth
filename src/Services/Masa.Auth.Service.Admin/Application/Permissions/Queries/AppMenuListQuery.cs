// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record AppMenuListQuery(string AppId, Guid UserId) : Query<List<MenuDto>>
{
    public override List<MenuDto> Result { get; set; } = new();
}
