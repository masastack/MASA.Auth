// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleUsersQuery(Guid RoleId, int Page, int PageSize) : Query<PaginationDto<UserSelectModel>>
{
    public override PaginationDto<UserSelectModel> Result { get; set; } = new();
}

