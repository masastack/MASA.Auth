// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.GlobalNavs;

public class CommandHandler
{
    readonly IGlobalNavVisibleRepository _repository;

    public CommandHandler(IGlobalNavVisibleRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task SaveAppGlobalNavVisibleAsync(SaveAppGlobalNavVisibleCommand command)
    {
        var dto = command.visibleDto;
        await _repository.RemoveAsync(x => x.AppId == dto.AppId);
        switch (dto.VisibleType)
        {
            case GlobalNavVisibleTypes.AllVisible:
                await _repository.AddAsync(new GlobalNavVisible(dto.AppId, string.Empty, true));
                break;
            case GlobalNavVisibleTypes.AllInvisible:
                await _repository.AddAsync(new GlobalNavVisible(dto.AppId, string.Empty, false));
                break;
            case GlobalNavVisibleTypes.Client:
                //批量入 change tracker，UoW 统一提交，避免逐条 await
                await _repository.AddRangeAsync(dto.ClientIds.Where(x => !string.IsNullOrEmpty(x))
                    .Select(clientId => new GlobalNavVisible(dto.AppId, clientId, true))
                    .ToList());
                break;
        }
    }
}
