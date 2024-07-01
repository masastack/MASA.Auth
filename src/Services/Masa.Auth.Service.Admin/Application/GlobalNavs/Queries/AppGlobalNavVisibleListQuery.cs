// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.GlobalNavs.Queries;

public record AppGlobalNavVisibleListQuery(IEnumerable<string> AppIds) : Query<List<AppGlobalNavVisibleDto>>
{
    public override List<AppGlobalNavVisibleDto> Result { get; set; } = new();
}
