namespace Masa.Auth.Service.Admin.Services;

public class ClientSerivce : ServiceBase
{
    public ClientSerivce(IServiceCollection services) : base(services, "api/sso/client")
    {
        MapGet(GetListAsync);
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

    private async Task<List<ClientTypeDetailDto>> GetClientTypeListAsync(IEventBus eventBus)
    {
        var query = new ClientTypeListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddClientAsync(IEventBus eventBus, [FromBody] ClientAddDto clientAddDto)
    {
        var addCommand = new AddClientCommand(clientAddDto);
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
