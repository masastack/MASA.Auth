// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record FavoriteMenuListQuery(Guid UserId) : Query<List<SelectItemDto<Guid>>>
{
    public override List<SelectItemDto<Guid>> Result { get; set; } = new();
}
