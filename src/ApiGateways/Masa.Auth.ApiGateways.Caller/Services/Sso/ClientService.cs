using Masa.Auth.Contracts.Admin.Sso;

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
        return await GetAsync<GetClientPaginationDto, PaginationDto<ClientDto>>("GetList", clientPaginationDto);
    }
}
