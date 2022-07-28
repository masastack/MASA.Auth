// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record StaffSelectQuery(string? Search) : Query<List<StaffSelectDto>>
{
    public int MaxCount { get; set; } = 20;

    public override List<StaffSelectDto> Result { get; set; } = new();
}
