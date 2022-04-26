namespace Masa.Auth.ApiGateways.Caller.Services.Sso;

public class ClientService : ServiceBase
{
    protected override string BaseUrl { get; set; } = "";

    public ClientService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/client";
    }

    public async Task<PaginationDto<ClientDto>> GetListAsync(GetClientPaginationDto clientPaginationDto)
    {
        return await GetAsync<PaginationDto<ClientDto>>("GetList", clientPaginationDto);
    }

    public async Task<List<ClientTypeDetailDto>> GetClientTypeListAsync()
    {
        return await GetAsync<List<ClientTypeDetailDto>>("GetClientTypeList");
    }

    public async Task AddClientAsync(ClientAddDto clientAddDto)
    {
        await PostAsync(nameof(AddClientAsync), clientAddDto);
    }

    public async Task UpdateClientAsync(ClientDetailDto clientDetailDto)
    {
        await PostAsync(nameof(AddClientAsync), clientDetailDto);
    }

    public async Task RemoveClientAsync(int id)
    {
        await DeleteAsync($"{nameof(RemoveClientAsync)}?id={id}");
    }
}
