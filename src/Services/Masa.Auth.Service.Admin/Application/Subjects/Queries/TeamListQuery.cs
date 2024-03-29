﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamListQuery(string Name, Guid UserId = default) : Query<List<TeamDto>>
{
    public override List<TeamDto> Result { get; set; } = new();
}
