// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record UserClaimSelectQuery(string? Search) : Query<List<UserClaimSelectDto>>
{
    public override List<UserClaimSelectDto> Result { get; set; } = new();
}
