// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentTreeQuery(Guid ParentId) : Query<List<DepartmentDto>>
{
    public override List<DepartmentDto> Result { get; set; } = new List<DepartmentDto>();
}

