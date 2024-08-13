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
            AppId = query.AppId,
            ClientIds = list.Select(x => x.ClientId).ToList(),
        };

        SetVisibleType(dto, list);

        query.Result = dto;
    }

    [EventHandler]
    public async Task GetAppGlobalNavVisibleListAsync(AppGlobalNavVisibleListQuery query)
    {
        var list = await _repository.GetListAsync(x => query.AppIds.Contains(x.AppId));

        var visibleDtos = list.GroupBy(x => x.AppId).Select(x =>
        {
            var dto = new AppGlobalNavVisibleDto
            {
                AppId = x.Key,
                ClientIds = x.Select(x => x.ClientId).ToList(),
            };
            SetVisibleType(dto, x);
            return dto;
        }).ToList();

        query.Result = visibleDtos;
    }

    private void SetVisibleType(AppGlobalNavVisibleDto dto, IEnumerable<GlobalNavVisible> list)
    {
        if (list.Any(x => !x.ClientId.IsNullOrEmpty() && x.Visible))
        {
            dto.VisibleType = GlobalNavVisibleTypes.Client;
        }
        else if (list.Any(x => x.ClientId.IsNullOrEmpty() && !x.Visible))
        {
            dto.VisibleType = GlobalNavVisibleTypes.AllInvisible;
        }
        else
        {
            dto.VisibleType = GlobalNavVisibleTypes.AllVisible;
        }
    }
}
