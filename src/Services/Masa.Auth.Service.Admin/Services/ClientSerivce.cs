// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ClientSerivce : ServiceBase
{
    public ClientSerivce() : base("api/sso/client")
    {
        MapGet(GetListAsync);
        MapGet(GetDetailAsync);
        MapGet(GetClientTypeListAsync);
        MapGet(GetClientSelectAsync);
        MapGet(GetClientSelectForCustomLoginAsync);
        MapPost(AddClientAsync);
        MapPost(UpdateClientAsync);
        MapDelete(RemoveClientAsync);
        MapPost(SyncOidcAsync);
    }

    private async Task<PaginationDto<ClientDto>> GetListAsync(IEventBus eventBus, GetClientPaginationDto clientPaginationDto)
    {
        var query = new ClientPaginationListQuery(clientPaginationDto.Page, clientPaginationDto.PageSize, clientPaginationDto.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ClientDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ClientDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ClientTypeDetailDto>> GetClientTypeListAsync(IEventBus eventBus)
    {
        var query = new ClientTypeListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<ClientSelectDto>> GetClientSelectAsync(IEventBus eventBus)
    {
        var query = new ClientSelectQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<ClientSelectDto>> GetClientSelectForCustomLoginAsync(IEventBus eventBus)
    {
        var query = new ClientSelectForCustomLoginQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddClientAsync(IEventBus eventBus, [FromBody] AddClientDto addClientDto)
    {
        var addCommand = new AddClientCommand(addClientDto);
        await eventBus.PublishAsync(addCommand);
    }

    private async Task UpdateClientAsync(IEventBus eventBus, [FromBody] ClientDetailDto clientDetailDto)
    {
        var updateCommand = new UpdateClientCommand(clientDetailDto);
        await eventBus.PublishAsync(updateCommand);
    }

    private async Task RemoveClientAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var removeCommand = new RemoveClientCommand(id);
        await eventBus.PublishAsync(removeCommand);
    }

    [AllowAnonymous]
    private async Task SyncOidcAsync(
        IEventBus eventBus)
    {
        var removeCommand = new SyncOidcCommand();
        await eventBus.PublishAsync(removeCommand);
    }
}
