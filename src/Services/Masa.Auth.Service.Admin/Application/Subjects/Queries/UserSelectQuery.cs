// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserSelectQuery(string Search, int Page = 1, int PageSize = 10) : Query<List<UserSelectAutoCompleteDto>>
{
    public override List<UserSelectAutoCompleteDto> Result { get; set; } = new();
}
