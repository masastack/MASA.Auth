// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffsQuery(int Page, int PageSize, string Search, bool? Enabled = true, Guid DepartmentId = default(Guid)) : Query<PaginationDto<StaffDto>>
{
    public override PaginationDto<StaffDto> Result { get; set; } = new();
}
