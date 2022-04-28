namespace Masa.Auth.Service.Admin.Application.Sso;

public class QueryHandler
{
    readonly ISsoClientRepository _ssoClientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;
    readonly AuthDbContext _authDbContext;

    public QueryHandler(ISsoClientRepository ssoClientRepository, IIdentityResourceRepository identityResourceRepository, AuthDbContext authDbContext)
    {
        _ssoClientRepository = ssoClientRepository;
        _identityResourceRepository = identityResourceRepository;
        _authDbContext = authDbContext;
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

    #region IdentityResource

    [EventHandler]
    public async Task GetIdentityResourceAsync(IdentityResourceQuery query)
    {
        Expression<Func<IdentityResource, bool>> condition = idrs => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(idrs => idrs.DisplayName.Contains(query.Search) || idrs.Name.Contains(query.Search));

        var identityResources = await _identityResourceRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(IdentityResource.ModificationTime)] = true,
                [nameof(IdentityResource.CreationTime)] = true,
            }
        });

        query.Result = new(identityResources.Total, identityResources.Result.Select(idrs =>
            new IdentityResourceDto(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable)
        ).ToList());
    }

    [EventHandler]
    public async Task GetIdentityResourceDetailAsync(IdentityResourceDetailQuery query)
    {
        var idrs = await _identityResourceRepository.GetDetailAsync(query.IdentityResourceId);
        if (idrs is null) throw new UserFriendlyException("This identityResource data does not exist");

        query.Result = new(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable, idrs.UserClaims.Select(u => u.Id).ToList(), idrs.Properties.ToDictionary(p => p.Key, p => p.Value));
    }

    [EventHandler]
    public async Task GetIdentityResourceSelectAsync(IdentityResourceSelectQuery query)
    {
        var idrs = await _authDbContext.Set<IdentityResource>()
                                .Select(idrs => new IdentityResourceSelectDto(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description))
                                .ToListAsync();

        query.Result = idrs;
    }

    #endregion
}
