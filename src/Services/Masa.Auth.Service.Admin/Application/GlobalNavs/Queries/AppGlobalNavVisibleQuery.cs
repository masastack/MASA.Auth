// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.GlobalNavs.Queries;

public record AppGlobalNavVisibleQuery(string AppId) : Query<AppGlobalNavVisibleDto>
{
    public override AppGlobalNavVisibleDto Result { get; set; } = new();
}
