namespace Masa.Auth.Service.Admin.Services;

public class ClientSerivce : ServiceBase
{
    public ClientSerivce(IServiceCollection services) : base(services, "api/sso/client")
    {
        MapGet(GetListAsync);
    }

    private async Task<PaginationDto<ClientDto>> GetListAsync(IEventBus eventBus, GetClientPaginationDto clientPaginationDto)
    {
        var query = new ClientPaginationListQuery(clientPaginationDto.Page, clientPaginationDto.PageSize);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
