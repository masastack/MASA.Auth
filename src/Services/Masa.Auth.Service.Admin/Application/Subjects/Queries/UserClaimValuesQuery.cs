// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserClaimValuesQuery(Guid UserId) : Query<List<ClaimValue>>
{
    public override List<ClaimValue> Result { get; set; } = new();
}