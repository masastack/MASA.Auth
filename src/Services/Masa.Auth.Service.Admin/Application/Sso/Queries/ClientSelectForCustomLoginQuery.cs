// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientSelectForCustomLoginQuery() : Query<List<ClientSelectDto>>
{
    public override List<ClientSelectDto> Result { get; set; } = new();
}
