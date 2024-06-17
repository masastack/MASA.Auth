// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.GlobalNavs;

public class QueryHandler
{
    readonly IGlobalNavVisibleRepository _repository;

    public QueryHandler(IGlobalNavVisibleRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task GetAppGlobalNavVisibleAsync(AppGlobalNavVisibleQuery query)
    {
        var list = await _repository.GetListAsync(x => x.AppId == query.AppId);

        var dto = new AppGlobalNavVisibleDto
        {
            ClientIds = list.Select(x => x.ClientId).ToList(),
        };
        
        if (list.Any(x => !x.ClientId.IsNullOrEmpty() && x.Visible))
        {
            dto.VisibleType = GlobalNavVisibleTypes.Client;
        }
        else if (!list.Any(x => !x.Visible))
        {
            dto.VisibleType = GlobalNavVisibleTypes.AllVisible;
        }
        else if (!list.Any(x => x.Visible))
        {
            dto.VisibleType = GlobalNavVisibleTypes.AllInvisible;
        }

        query.Result = dto;
    }
}
