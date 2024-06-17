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

    public async Task SaveAppGlobalNavVisibleAsync(SaveAppGlobalNavVisibleCommand command)
    {
        var dto = command.visibleDto;

        switch (dto.VisibleType)
        {
            case GlobalNavVisibleTypes.AllVisible:
                await _repository.AddAsync(new GlobalNavVisible(dto.AppId, string.Empty, true));
                break;
            case GlobalNavVisibleTypes.AllInvisible:
                await _repository.AddAsync(new GlobalNavVisible(dto.AppId, string.Empty, false));
                break;
            case GlobalNavVisibleTypes.Client:
                foreach (var item in dto.ClientIds)
                {
                    await _repository.AddAsync(new GlobalNavVisible(dto.AppId, item, true));
                }
                break;
        }
    }
}
