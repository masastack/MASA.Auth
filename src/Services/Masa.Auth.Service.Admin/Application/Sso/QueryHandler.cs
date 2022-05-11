// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

public class QueryHandler
{
    readonly IClientRepository _clientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;
    readonly IApiResourceRepository _apiResourceRepository;
    readonly IApiScopeRepository _apiScopeRepository;
    readonly IUserClaimRepository _userClaimRepository;
    readonly ICustomLoginRepository _customLoginRepository;
    readonly AuthDbContext _authDbContext;

    public QueryHandler(IClientRepository clientRepository, IIdentityResourceRepository identityResourceRepository, IApiResourceRepository apiResourceRepository, IApiScopeRepository apiScopeRepository, IUserClaimRepository userClaimRepository, ICustomLoginRepository customLoginRepository, AuthDbContext authDbContext)
    {
        _clientRepository = clientRepository;
        _identityResourceRepository = identityResourceRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
        _userClaimRepository = userClaimRepository;
        _customLoginRepository = customLoginRepository;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    public async Task ClientPaginationListAsync(ClientPaginationListQuery clientPaginationListQuery)
    {
        var result = await _clientRepository.GetPaginatedListAsync(new PaginatedOptions
        {
            Page = clientPaginationListQuery.Page,
            PageSize = clientPaginationListQuery.PageSize
        });
        clientPaginationListQuery.Result = new PaginationDto<ClientDto>(result.Total, result.Result.Adapt<List<ClientDto>>());
    }

    [EventHandler]
    public async Task ClientDetailQueryAsync(ClientDetailQuery clientDetailQuery)
    {
        var client = await _clientRepository.GetByIdAsync(clientDetailQuery.ClientId);
        client.Adapt(clientDetailQuery.Result);
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
                    //todo: i18n
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
                }
            }).ToList();
    }

    [EventHandler]
    public async Task GetClientSelectAsync(ClientSelectQuery query)
    {
        query.Result = await _authDbContext.Set<Client>().Select(client => new ClientSelectDto(client.Id, client.ClientName, client.LogoUri, client.Description, client.ClientId, client.ClientType)).ToListAsync();
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

        query.Result = new(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable, idrs.UserClaims.Select(u => u.UserClaimId).ToList(), idrs.Properties.ToDictionary(p => p.Key, p => p.Value));
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

    #region ApiResource

    [EventHandler]
    public async Task GetApiResourceAsync(ApiResourceQuery query)
    {
        Expression<Func<ApiResource, bool>> condition = apiResource => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(apiResource => apiResource.DisplayName.Contains(query.Search) || apiResource.Name.Contains(query.Search));

        var apiResources = await _apiResourceRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ApiResource.ModificationTime)] = true,
                [nameof(ApiResource.CreationTime)] = true,
            }
        });

        query.Result = new(apiResources.Total, apiResources.Result.Select(apiResource => (ApiResourceDto)apiResource).ToList());
    }

    [EventHandler]
    public async Task GetApiResourceDetailAsync(ApiResourceDetailQuery query)
    {
        var apiResource = await _apiResourceRepository.GetDetailAsync(query.ApiResourceId);
        if (apiResource is null) throw new UserFriendlyException("This apiResource data does not exist");

        query.Result = apiResource;
    }

    [EventHandler]
    public async Task GetApiResourceSelectAsync(ApiResourceSelectQuery query)
    {
        var apiResourceSelect = await _authDbContext.Set<ApiResource>()
                                .OrderByDescending(apiResource => apiResource.ModificationTime)
                                .ThenByDescending(apiResource => apiResource.CreationTime)
                                .Select(apiResource => new ApiResourceSelectDto(apiResource.Id, apiResource.Name, apiResource.DisplayName, apiResource.Description))                                
                                .ToListAsync();

        query.Result = apiResourceSelect;
    }

    #endregion

    #region ApiScope

    [EventHandler]
    public async Task GetApiScopeAsync(ApiScopeQuery query)
    {
        Expression<Func<ApiScope, bool>> condition = apiScopes => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(apiScope => apiScope.DisplayName.Contains(query.Search) || apiScope.Name.Contains(query.Search));

        var apiScopes = await _apiScopeRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ApiScope.ModificationTime)] = true,
                [nameof(ApiScope.CreationTime)] = true,
            }
        });

        query.Result = new(apiScopes.Total, apiScopes.Result.Select(apiScope => (ApiScopeDto)apiScope).ToList());
    }

    [EventHandler]
    public async Task GetApiScopeDetailAsync(ApiScopeDetailQuery query)
    {
        var apiScope = await _apiScopeRepository.GetDetailAsync(query.ApiScopeId);
        if (apiScope is null) throw new UserFriendlyException("This apiScope data does not exist");

        query.Result = apiScope;
    }

    [EventHandler]
    public async Task GetApiScopeSelectAsync(ApiScopeSelectQuery query)
    {
        var apiScopeSelect = await _authDbContext.Set<ApiScope>()
                                .OrderByDescending(apiScope => apiScope.ModificationTime)
                                .ThenByDescending(apiScope => apiScope.CreationTime)
                                .Select(apiScope => new ApiScopeSelectDto(apiScope.Id, apiScope.Name, apiScope.DisplayName, apiScope.Description))
                                .ToListAsync();

        query.Result = apiScopeSelect;
    }

    #endregion

    #region UserClaim

    [EventHandler]
    public async Task GetUserClaimAsync(UserClaimQuery query)
    {
        Expression<Func<UserClaim, bool>> condition = userClaim => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(userClaim => userClaim.Name.Contains(query.Search));

        var userClaims = await _userClaimRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(UserClaim.ModificationTime)] = true,
                [nameof(UserClaim.CreationTime)] = true,
            }
        });

        query.Result = new(userClaims.Total, userClaims.Result.Select(userClaim => (UserClaimDto)userClaim).ToList());
    }

    [EventHandler]
    public async Task GetUserClaimDetailAsync(UserClaimDetailQuery query)
    {
        var userClaim = await _userClaimRepository.FindAsync(userClaim => userClaim.Id == query.UserClaimId);
        if (userClaim is null) throw new UserFriendlyException("This userClaim data does not exist");

        query.Result = userClaim;
    }

    [EventHandler]
    public async Task GetUserClaimSelectAsync(UserClaimSelectQuery query)
    {
        var userClaimSelect = await _authDbContext.Set<UserClaim>()
                                .OrderByDescending(userClaim => userClaim.ModificationTime)
                                .ThenByDescending(userClaim => userClaim.CreationTime)
                                .Select(userClaim => new UserClaimSelectDto(userClaim.Id, userClaim.Name, userClaim.Description, userClaim.UserClaimType))
                                .ToListAsync();

        query.Result = userClaimSelect;
    }

    #endregion

    #region CustomLogin

    [EventHandler]
    public async Task GetCustomLoginAsync(CustomLoginQuery query)
    {
        Expression<Func<CustomLogin, bool>> condition = customLogin => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(customLogin => customLogin.Name.Contains(query.Search));

        var customLoginQuery = _authDbContext.Set<CustomLogin>().Where(condition);
        var total = await customLoginQuery.LongCountAsync();
        var customLogins = await customLoginQuery.Include(customLogin => customLogin.Client)
                                   .OrderByDescending(s => s.ModificationTime)
                                   .ThenByDescending(s => s.CreationTime)
                                   .Skip((query.Page - 1) * query.PageSize)
                                   .Take(query.PageSize)
                                   .ToListAsync();

        query.Result = new(total, customLogins.Select(customLogin => (CustomLoginDto)customLogin).ToList());
    }

    [EventHandler]
    public async Task GetCustomLoginDetailAsync(CustomLoginDetailQuery query)
    {
        var customLogin = await _customLoginRepository.GetDetailAsync(query.CustomLoginId);
        if (customLogin is null) throw new UserFriendlyException("This customLogin data does not exist");

        query.Result = customLogin;
    }

    #endregion
}
