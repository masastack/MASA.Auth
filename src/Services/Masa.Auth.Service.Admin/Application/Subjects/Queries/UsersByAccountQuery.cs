// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UsersByAccountQuery(IEnumerable<string> Accounts) : Query<List<UserSimpleModel>>
{
    public override List<UserSimpleModel> Result { get; set; } = new();
}

