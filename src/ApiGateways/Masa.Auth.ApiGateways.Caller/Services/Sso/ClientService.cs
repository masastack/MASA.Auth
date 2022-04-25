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
        var paramters = new Dictionary<string, string>
        {
            ["pageSize"] = clientPaginationDto.PageSize.ToString(),
            ["page"] = clientPaginationDto.Page.ToString(),
            ["search"] = clientPaginationDto.Search ?? "",
        };
        return await GetAsync<PaginationDto<ClientDto>>("GetList", paramters);
    }
}
