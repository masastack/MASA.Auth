// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UsersQuery(int Page, int PageSize, Guid UserId, bool? Enabled, DateTime? StartTime, DateTime? EndTime) : Query<PaginationDto<UserDto>>
{
    public override PaginationDto<UserDto> Result { get; set; } = new();
}
