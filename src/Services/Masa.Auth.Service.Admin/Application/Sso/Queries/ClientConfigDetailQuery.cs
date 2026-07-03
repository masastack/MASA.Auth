// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientConfigDetailQuery(string ClientId) : Query<ClientConfigDto>
{
    public override ClientConfigDto Result { get; set; } = new();
}
