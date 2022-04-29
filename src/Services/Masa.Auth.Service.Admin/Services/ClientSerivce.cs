// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ClientSerivce : ServiceBase
{
    public ClientSerivce(IServiceCollection services) : base(services, "api/sso/client")
    {
        MapGet(GetListAsync);
        MapGet(GetDetailAsync);
        MapGet(GetClientTypeListAsync);
        MapPost(AddClientAsync);
        MapPost(UpdateClientAsync);
        MapDelete(RemoveClientAsync);
    }

    private async Task<PaginationDto<ClientDto>> GetListAsync(IEventBus eventBus, GetClientPaginationDto clientPaginationDto)
    {
        var query = new ClientPaginationListQuery(clientPaginationDto.Page, clientPaginationDto.PageSize);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ClientDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] int id)
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

    private async Task RemoveClientAsync(IEventBus eventBus, [FromQuery] int id)
    {
        var removeCommand = new RemoveClientCommand(id);
        await eventBus.PublishAsync(removeCommand);
    }
}
