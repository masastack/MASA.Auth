// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

public class QueryHandler
{
    private readonly IClientRepository _clientRepository;
    private readonly IIdentityResourceRepository _identityResourceRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IApiScopeRepository _apiScopeRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly ICustomLoginRepository _customLoginRepository;
    private readonly DbContext _oidcDbContext;
    private readonly AuthDbContext _authDbContext;
    private readonly IClientCache _clientCache;
    private readonly OperaterProvider _operaterProvider;

    public QueryHandler(
        IClientRepository clientRepository,
        IIdentityResourceRepository identityResourceRepository,
        IApiResourceRepository apiResourceRepository,
        IApiScopeRepository apiScopeRepository,
        IUserClaimRepository userClaimRepository,
        ICustomLoginRepository customLoginRepository,
        OidcDbContext oidcDbContext,
        AuthDbContext authDbContext,
        IClientCache clientCache,
        OperaterProvider operaterProvider)
    {
        _clientRepository = clientRepository;
        _identityResourceRepository = identityResourceRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
        _userClaimRepository = userClaimRepository;
        _customLoginRepository = customLoginRepository;
        _oidcDbContext = oidcDbContext;
        _authDbContext = authDbContext;
        _clientCache = clientCache;
        _operaterProvider = operaterProvider;
    }

    #region Client

    [EventHandler]
    public async Task ClientPaginationListAsync(ClientPaginationListQuery clientPaginationListQuery)
    {
        var searchKey = clientPaginationListQuery.SearchKey;
        Expression<Func<Client, bool>> condition = client => true;
        if (!string.IsNullOrWhiteSpace(searchKey))
            condition = condition.And(client => client.ClientName.Contains(searchKey) || client.ClientId.Contains(searchKey));

        var result = await _clientRepository.GetPaginatedListAsync(clientPaginationListQuery.Page, clientPaginationListQuery.PageSize, condition);
        clientPaginationListQuery.Result = new PaginationDto<ClientDto>(result.Total, result.Result.Adapt<List<ClientDto>>());
    }

    [EventHandler]
    public async Task ClientDetailQueryAsync(ClientDetailQuery clientDetailQuery)
    {
        var client = await _clientRepository.GetDetailAsync(clientDetailQuery.ClientId);
        client.Adapt(clientDetailQuery.Result);
    }

    [EventHandler]
    public void ClientTypeListAsync(ClientTypeListQuery clientTypeListQuery)
    {
        clientTypeListQuery.Result = EnumUtil.GetItems<ClientTypes>()
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
        query.Result = await _oidcDbContext.Set<Client>().Select(client => new ClientSelectDto(client.Id, client.ClientName, client.LogoUri, client.Description, client.ClientId, client.ClientType)).ToListAsync();
    }

    [EventHandler]
    public async Task GetClientSelectForCustomLoginAsync(ClientSelectForCustomLoginQuery query)
    {
        var alreadyUseClientIds = await _authDbContext.Set<CustomLogin>()
                                                     .Where(customLogin => customLogin.Enabled)
                                                     .Select(customLogin => customLogin.ClientId)
                                                     .ToListAsync();

        query.Result = await _oidcDbContext.Set<Client>()
                                           .Where(client => alreadyUseClientIds.Contains(client.ClientId) == false)
                                           .Select(client => new ClientSelectDto(client.Id, client.ClientName, client.LogoUri, client.Description, client.ClientId, client.ClientType))
                                           .ToListAsync();
    }

    #endregion

    #region IdentityResource

    [EventHandler]
    public async Task GetIdentityResourceAsync(IdentityResourcesQuery query)
    {
        Expression<Func<IdentityResource, bool>> condition = idrs => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(idrs => idrs.DisplayName.Contains(query.Search) || idrs.Name.Contains(query.Search));

        var identityResources = await _identityResourceRepository.GetPaginatedListAsync(query.Page, query.PageSize, condition);

        query.Result = new(identityResources.Total, identityResources.Result.Select(idrs =>
            new IdentityResourceDto(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable)
        ).ToList());
    }

    [EventHandler]
    public async Task GetIdentityResourceDetailAsync(IdentityResourceDetailQuery query)
    {
        var idrs = await _identityResourceRepository.GetDetailAsync(query.IdentityResourceId);
        if (idrs is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_RESOURCE_NOT_EXIST);

        query.Result = new(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable, idrs.UserClaims.Select(u => u.UserClaimId).ToList(), idrs.Properties.ToDictionary(p => p.Key, p => p.Value));
    }

    [EventHandler]
    public async Task GetIdentityResourceSelectAsync(IdentityResourceSelectQuery query)
    {
        var idrs = await _oidcDbContext.Set<IdentityResource>()
                                .Where(idrs => idrs.Enabled)
                                .OrderByDescending(idrs => idrs.ModificationTime)
                                .ThenByDescending(idrs => idrs.CreationTime)
                                .Select(idrs => new IdentityResourceSelectDto(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description))
                                .ToListAsync();

        query.Result = idrs;
    }

    #endregion

    #region ApiResource

    [EventHandler]
    public async Task GetApiResourceAsync(ApiResourcesQuery query)
    {
        Expression<Func<ApiResource, bool>> condition = apiResource => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(apiResource => apiResource.DisplayName.Contains(query.Search) || apiResource.Name.Contains(query.Search));

        var apiResources = await _apiResourceRepository.GetPaginatedListAsync(query.Page, query.PageSize, condition);

        query.Result = new(apiResources.Total, apiResources.Result.Select(apiResource => new ApiResourceDto()
        {
            Id = apiResource.Id,
            Enabled = apiResource.Enabled,
            Name = apiResource.Name,
            DisplayName = apiResource.DisplayName,
            Description = apiResource.Description,
            AllowedAccessTokenSigningAlgorithms = apiResource.AllowedAccessTokenSigningAlgorithms,
            ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
            LastAccessed = apiResource.LastAccessed,
            NonEditable = apiResource.NonEditable
        }).ToList());
    }

    [EventHandler]
    public async Task GetApiResourceDetailAsync(ApiResourceDetailQuery query)
    {
        var apiResource = await _apiResourceRepository.GetDetailAsync(query.ApiResourceId);
        if (apiResource is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_RESOURCE_NOT_EXIST);

        query.Result = new ApiResourceDetailDto()
        {
            Id = apiResource.Id,
            Enabled = apiResource.Enabled,
            Name = apiResource.Name,
            DisplayName = apiResource.DisplayName,
            Description = apiResource.Description,
            AllowedAccessTokenSigningAlgorithms = apiResource.AllowedAccessTokenSigningAlgorithms,
            ShowInDiscoveryDocument = apiResource.ShowInDiscoveryDocument,
            LastAccessed = apiResource.LastAccessed,
            NonEditable = apiResource.NonEditable,
            ApiScopes = apiResource.ApiScopes.Select(a => a.ApiScopeId).ToList(),
            UserClaims = apiResource.UserClaims.Select(u => u.UserClaimId).ToList(),
            Properties = apiResource.Properties.ToDictionary(p => p.Key, p => p.Value),
            Secrets = apiResource.Secrets.Select(s => s.Value).ToList()
        };
    }

    [EventHandler]
    public async Task GetApiResourceSelectAsync(ApiResourceSelectQuery query)
    {
        var apiResourceSelect = await _oidcDbContext.Set<ApiResource>()
                                .Where(apiResource => apiResource.Enabled == true)
                                .OrderByDescending(apiResource => apiResource.ModificationTime)
                                .ThenByDescending(apiResource => apiResource.CreationTime)
                                .Select(apiResource => new ApiResourceSelectDto(apiResource.Id, apiResource.Name, apiResource.DisplayName, apiResource.Description))
                                .ToListAsync();

        query.Result = apiResourceSelect;
    }

    #endregion

    #region ApiScope

    [EventHandler]
    public async Task GetApiScopeAsync(ApiScopesQuery query)
    {
        Expression<Func<ApiScope, bool>> condition = apiScopes => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(apiScope => apiScope.DisplayName.Contains(query.Search) || apiScope.Name.Contains(query.Search));

        var apiScopes = await _apiScopeRepository.GetPaginatedListAsync(query.Page, query.PageSize, condition);

        query.Result = new(apiScopes.Total, apiScopes.Result.Select(apiScope => new ApiScopeDto()
        {
            Id = apiScope.Id,
            Name = apiScope.Name,
            Enabled = apiScope.Enabled,
            DisplayName = apiScope.DisplayName,
            Description = apiScope.Description,
            Required = apiScope.Required,
            Emphasize = apiScope.Emphasize,
            ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument
        }).ToList());
    }

    [EventHandler]
    public async Task GetApiScopeDetailAsync(ApiScopeDetailQuery query)
    {
        var apiScope = await _apiScopeRepository.GetDetailAsync(query.ApiScopeId);
        if (apiScope is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_SCOPE_NOT_EXIST);

        query.Result = new ApiScopeDetailDto()
        {
            Id = apiScope.Id,
            Name = apiScope.Name,
            DisplayName = apiScope.DisplayName,
            Description = apiScope.Description,
            Required = apiScope.Required,
            Emphasize = apiScope.Emphasize,
            Enabled = apiScope.Enabled,
            ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
            UserClaims = apiScope.UserClaims.Select(u => u.UserClaimId).ToList(),
            Properties = apiScope.Properties.ToDictionary(p => p.Key, p => p.Value)
        };
    }

    [EventHandler]
    public async Task GetApiScopeSelectAsync(ApiScopeSelectQuery query)
    {
        var apiScopeSelect = await _oidcDbContext.Set<ApiScope>()
                                .Where(apiScope => apiScope.Enabled)
                                .OrderByDescending(apiScope => apiScope.ModificationTime)
                                .ThenByDescending(apiScope => apiScope.CreationTime)
                                .Select(apiScope => new ApiScopeSelectDto(apiScope.Id, apiScope.Name, apiScope.DisplayName, apiScope.Description))
                                .ToListAsync();

        query.Result = apiScopeSelect;
    }

    #endregion

    #region UserClaim

    [EventHandler]
    public async Task GetUserClaimAsync(UserClaimsQuery query)
    {
        Expression<Func<UserClaim, bool>> condition = userClaim => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(userClaim => userClaim.Name.Contains(query.Search));

        var userClaims = await _userClaimRepository.GetPaginatedListAsync(query.Page, query.PageSize, condition);

        query.Result = new(userClaims.Total, userClaims.Result.Select(userClaim => new UserClaimDto()
        {
            Id = userClaim.Id,
            Name = userClaim.Name,
            Description = userClaim.Description,
            UserClaimType = StandardUserClaims.Claims.ContainsKey(userClaim.Name) ? UserClaimType.Standard : UserClaimType.Customize
        }).ToList());
    }

    [EventHandler]
    public async Task GetUserClaimDetailAsync(UserClaimDetailQuery query)
    {
        var userClaim = await _userClaimRepository.GetDetailAsync(query.UserClaimId);
        if (userClaim is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_CLAIM_NOT_EXIST);

        query.Result = new UserClaimDetailDto()
        {
            Id = userClaim.Id,
            Name = userClaim.Name,
            Description = userClaim.Description,
            UserClaimType = StandardUserClaims.Claims.ContainsKey(userClaim.Name) ? UserClaimType.Standard : UserClaimType.Customize
        };
    }

    [EventHandler]
    public async Task GetUserClaimSelectAsync(UserClaimSelectQuery query)
    {
        var userClaimSelect = await _oidcDbContext.Set<UserClaim>()
                                .OrderByDescending(userClaim => userClaim.ModificationTime)
                                .ThenByDescending(userClaim => userClaim.CreationTime)
                                .Select(userClaim => new UserClaimSelectDto(userClaim.Id, userClaim.Name, userClaim.Description))
                                .ToListAsync();
        userClaimSelect.ForEach(userClaim =>
        {
            if (StandardUserClaims.Claims.ContainsKey(userClaim.Name)) userClaim.UserClaimType = UserClaimType.Standard;
            else userClaim.UserClaimType = UserClaimType.Customize;
        });
        query.Result = userClaimSelect;
    }

    #endregion

    #region CustomLogin

    [EventHandler]
    public async Task GetCustomLoginAsync(CustomLoginsQuery query)
    {
        Expression<Func<CustomLogin, bool>> condition = customLogin => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(customLogin => customLogin.Name.Contains(query.Search));

        var customLoginQuery = _authDbContext.Set<CustomLogin>().Where(condition);
        var total = await customLoginQuery.LongCountAsync();
        var customLogins = await customLoginQuery.OrderByDescending(s => s.ModificationTime)
                                           .ThenByDescending(s => s.CreationTime)
                                           .Skip((query.Page - 1) * query.PageSize)
                                           .Take(query.PageSize)
                                           .ToListAsync();

        var clients = await _clientCache.GetListAsync(customLogins.Select(customLogin => customLogin.ClientId));
        var customLoginDtos = customLogins.Select(customLogin =>
        {
            var (creator, modifier) = _operaterProvider.GetActionInfoAsync(customLogin.Creator, customLogin.Modifier).Result;
            var customLoginDto = new CustomLoginDto(customLogin.Id, customLogin.Name, customLogin.Title, new(), customLogin.Enabled, customLogin.CreationTime, customLogin.ModificationTime, creator, modifier);
            var client = clients.FirstOrDefault(client => client.ClientId == customLogin.ClientId);
            if (client is not null)
            {
                customLoginDto.Client = new ClientDto
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    LogoUri = client.LogoUri,
                    Enabled = client.Enabled,
                    Description = client.Description,
                };
            }
            return customLoginDto;
        }).ToList();

        query.Result = new(total, customLoginDtos);
    }

    [EventHandler]
    public async Task GetCustomLoginDetailAsync(CustomLoginDetailQuery query)
    {
        var customLogin = await _customLoginRepository.GetDetailAsync(query.CustomLoginId);
        if (customLogin is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CUSTOM_LOGIN_NOT_EXIST);

        var thirdPartyIdps = customLogin.ThirdPartyIdps.Select(tp => new CustomLoginThirdPartyIdpDto(tp.ThirdPartyIdpId, tp.Sort)).ToList();
        var registerFields = customLogin.RegisterFields.Select(rf => new RegisterFieldDto(rf.RegisterFieldType, rf.Sort, rf.Required)).ToList();
        var customLoginDetail = new CustomLoginDetailDto(customLogin.Id, customLogin.Name, customLogin.Title, new(), customLogin.Enabled, customLogin.CreationTime, customLogin.ModificationTime, customLogin.CreateUser?.Name ?? "", customLogin.ModifyUser?.Name ?? "", thirdPartyIdps, registerFields);
        var client = await _clientCache.GetAsync(customLogin.ClientId);
        if (client is not null)
        {
            customLoginDetail.Client = new ClientDto
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                LogoUri = client.LogoUri,
                Enabled = client.Enabled,
                Description = client.Description,
            };
        }

        query.Result = customLoginDetail;
    }

    [EventHandler]
    public async Task CustomLoginByClientIdQueryAsync(CustomLoginByClientIdQuery query)
    {
        var customLogin = await _authDbContext.Set<CustomLogin>()
                                                .Where(customLogin => customLogin.Enabled == true)
                                                .Include(customLogin => customLogin.ThirdPartyIdps)
                                                .ThenInclude(thirdPartyIdp => thirdPartyIdp.ThirdPartyIdp)
                                                .Include(customLogin => customLogin.RegisterFields)
                                                .FirstOrDefaultAsync(customLogin => customLogin.ClientId == query.ClientId);

        if (customLogin is null)
        {
            query.Result = new CustomLoginModel()
            {
                ClientId = query.ClientId,
                RegisterFields = new List<RegisterFieldModel>()
                {
                    new RegisterFieldModel
                    {
                        RegisterFieldType = RegisterFieldTypes.PhoneNumber
                    }
                }
            };
        }
        if (customLogin is not null)
        {
            query.Result = new CustomLoginModel()
            {
                Name = customLogin.Name,
                Title = customLogin.Title,
                ClientId = customLogin.ClientId,
                RegisterFields = customLogin.RegisterFields.Adapt<List<RegisterFieldModel>>(),
                ThirdPartyIdps = customLogin.ThirdPartyIdps
                                            .OrderBy(thirdPartyIdp => thirdPartyIdp.Sort)
                                            .Select(thirdPartyIdp => thirdPartyIdp.ThirdPartyIdp)
                                            .Where(thirdPartyIdp => thirdPartyIdp.Enabled)
                                            .Adapt<List<ThirdPartyIdpModel>>()
            };
        }
    }

    #endregion
}
