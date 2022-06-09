// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record PositionDetailQuery(Guid PositionId) : Query<PositionDetailDto>
{
    public override PositionDetailDto Result { get; set; } = new();
}
