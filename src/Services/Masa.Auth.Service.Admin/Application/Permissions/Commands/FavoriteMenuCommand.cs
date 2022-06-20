// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

<<<<<<<< HEAD:src/Services/Masa.Auth.Service.Admin/Application/Permissions/Commands/FavoriteMenuCommand.cs
public record FavoriteMenuCommand(Guid PermissionId, Guid UserId, bool IsFavorite) : Command
========
public record MenuFavoriteCommand(Guid PermissionId, Guid UserId, bool IsFavorite) : Command
>>>>>>>> 8ebc08268989050e8d227a04434fb0038ba37a4e:src/Services/Masa.Auth.Service.Admin/Application/Permissions/Commands/MenuFavoriteCommand.cs
{
}
