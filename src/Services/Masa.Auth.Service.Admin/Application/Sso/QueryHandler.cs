namespace Masa.Auth.Service.Admin.Application.Sso;

public class QueryHandler
{
    readonly IClientRepository _ssoClientRepository;
    public QueryHandler(IClientRepository ssoClientRepository)
    readonly ISsoClientRepository _ssoClientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;

    public QueryHandler(ISsoClientRepository ssoClientRepository, IIdentityResourceRepository identityResourceRepository)
    {
        _ssoClientRepository = ssoClientRepository;
        _identityResourceRepository = identityResourceRepository;
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
    }

    [EventHandler]
    public void ClientTypeListAsync(ClientTypeListQuery clientTypeListQuery)
    {
        clientTypeListQuery.Result = Enum<ClientTypes>.GetItems()
            .Select(ct => new ClientTypeDetailDto
            {
                ClientType = ct,
                Description = ct switch
                {
                    ClientTypes.Web => "Server-side applications where authentication and tokens are handled on the server (for example, Go, Java, ASP.Net, Node.js, PHP)",
                    ClientTypes.Native => "Desktop or mobile applications that run natively on a device and redirect users to a non-HTTP callback (for example, iOS, Android, React Native)",
                    ClientTypes.Spa => "Single-page web applications that run in the browser where the client receives tokens (for example, Javascript, Angular, React, Vue)",
                    ClientTypes.Device => "Input-constrained devices such as a smart TV, IoT device, or printer.",
                    ClientTypes.Machine => "Interact with APIs using the scoped OAuth 2.0 access tokens for machine-to-machine authentication.",
                    _ => ""
                },
                Icon = ct switch
                {
                    ClientTypes.Web => "mdi-file-code-outline",
                    ClientTypes.Native => "mdi-cellphone",
                    ClientTypes.Spa => "mdi-laptop",
                    ClientTypes.Device => "mdi-devices",
                    ClientTypes.Machine => "mdi-server",
                    _ => ""
                },
                GrantTypes = ct switch
                {
                    ClientTypes.Web => GrantTypeConsts.Code,
                    ClientTypes.Native => GrantTypeConsts.Code,
                    ClientTypes.Spa => GrantTypeConsts.Code,
                    ClientTypes.Device => GrantTypeConsts.DeviceFlow,
                    ClientTypes.Machine => GrantTypeConsts.ClientCredentials,
                    _ => new List<string>()
                }
            }).ToList();
    }

    #region IdentityResource

    [EventHandler]
    public async Task GetIdentityResourceAsync(IdentityResourcesQuery query)
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
        var idrs = await _identityResourceRepository.GetDetailByIdAsync(query.IdentityResourceId);
        if (idrs is null) throw new UserFriendlyException("This identityResource data does not exist");

        query.Result = new(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable, idrs.UserClaims.Select(u => u.Id).ToList(), idrs.Properties.ToDictionary(p => p.Key, p => p.Value));
    }

    [EventHandler]
    public async Task GetIdentityResourceSelectAsync(IdentityResourceSelectQuery query)
    {
        query.Result = await _identityResourceRepository.GetIdentityResourceSelect();
    }

    #endregion
}
