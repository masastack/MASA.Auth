namespace Masa.Auth.Service.Admin.Application.Sso;

public class QueryHandler
{
    readonly ISsoClientRepository _ssoClientRepository;
    public QueryHandler(ISsoClientRepository ssoClientRepository)
    {
        _ssoClientRepository = ssoClientRepository;
    }

    [EventHandler]
    public async Task ClientPaginationListAsync(ClientPaginationListQuery clientPaginationListQuery)
    {
        var result = await _ssoClientRepository.GetPaginatedListAsync(new PaginatedOptions
        {
            Page = clientPaginationListQuery.Page,
            PageSize = clientPaginationListQuery.PageSize
        });
        clientPaginationListQuery.Result = new PaginationDto<ClientDto>(result.Total, result.Result.Adapt<List<ClientDto>>());
        clientPaginationListQuery.Result = new PaginationDto<ClientDto>(5, new List<ClientDto> {
            new ClientDto(){ Id=1,ClientId="12333",Description="ddddddddddd",Enabled = true,ClientName="client_name",ClientType = ClientTypes.Web  },
            new ClientDto(){ Id=1,ClientId="12333",Description="ddddddddddd",Enabled = true,ClientName="client_name",ClientType = ClientTypes.Web  },
            new ClientDto(){ Id=1,ClientId="12333",Description="ddddddddddd",Enabled = true,ClientName="client_name",ClientType = ClientTypes.Web  },
            new ClientDto(){ Id=1,ClientId="12333",Description="ddddddddddd",Enabled = true,ClientName="client_name",ClientType = ClientTypes.Web  },
            new ClientDto(){ Id=1,ClientId="12333",Description="ddddddddddd",Enabled = true,ClientName="client_name",ClientType = ClientTypes.Web  }
        });
    }
}
