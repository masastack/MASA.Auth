// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

<<<<<<<< HEAD:src/Services/Masa.Auth.Service.Admin/Application/Permissions/Queries/FavoriteMenuListQuery.cs
public record FavoriteMenuListQuery(Guid UserId) : Query<List<SelectItemDto<Guid>>>
========
public record MenuFavoriteListQuery(Guid UserId) : Query<List<SelectItemDto<Guid>>>
>>>>>>>> 8ebc08268989050e8d227a04434fb0038ba37a4e:src/Services/Masa.Auth.Service.Admin/Application/Permissions/Queries/MenuFavoriteListQuery.cs
{
    public override List<SelectItemDto<Guid>> Result { get; set; } = new();
}
