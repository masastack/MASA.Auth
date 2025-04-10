// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles;

public class CommandHandler
{
    private readonly IDynamicRoleRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public CommandHandler(IDynamicRoleRepository repository, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task CreateAsync(CreateDynamicRoleCommand command)
    {
        var entity = command.UpsertDto.Adapt<DynamicRole>();
        await _repository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateDynamicRoleCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        command.UpsertDto.Adapt(entity);

        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteDynamicRoleCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("DynamicRole"));

        await _repository.RemoveAsync(entity);
    }
}
