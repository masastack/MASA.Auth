namespace Masa.Auth.Service.Admin.Application.Sso;

public class CommandHandler
{
    readonly IClientRepository _clientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;
    readonly IApiResourceRepository _apiResourceRepository;
    readonly IApiScopeRepository _apiScopeRepository;
    readonly IUserClaimRepository _userClaimRepository;

    public CommandHandler(IClientRepository clientRepository, IIdentityResourceRepository identityResourceRepository, IApiResourceRepository apiResourceRepository, IApiScopeRepository apiScopeRepository, IUserClaimRepository userClaimRepository)
    {
        _clientRepository = clientRepository;
        _identityResourceRepository = identityResourceRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
        _userClaimRepository = userClaimRepository;
    }



    #region Client
    [EventHandler]
    public async Task AddClientAsync(AddClientCommand addClientCommand)
    {
        PrepareGrantTypeWithClientType(addClientCommand.AddClientDto);
        var client = addClientCommand.AddClientDto.Adapt<Client>();
        await _clientRepository.AddAsync(client);

        void PrepareGrantTypeWithClientType(AddClientDto client)
        {
            switch (client.ClientType)
            {
                case ClientTypes.Web:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = true;
                    break;
                case ClientTypes.Spa:
                case ClientTypes.Native:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.Code);
                    client.RequirePkce = true;
                    client.RequireClientSecret = false;
                    break;
                case ClientTypes.Machine:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.ClientCredentials);
                    client.RequireClientSecret = true;
                    break;
                case ClientTypes.Device:
                    client.AllowedGrantTypes.AddRange(GrantTypeConsts.DeviceFlow);
                    client.RequireClientSecret = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [EventHandler]
    public async Task UpdateClientAsync(UpdateClientCommand updateClientCommand)
    {
        var id = updateClientCommand.ClientDetailDto.Id;
        updateClientCommand.ClientDetailDto.ClientSecrets.ForEach(secret =>
        {
            HashClientSharedSecret(secret);
        });
        var client = await _clientRepository.GetByIdAsync(id);
        //Contrary to DDD
        updateClientCommand.ClientDetailDto.Adapt(client);
        await _clientRepository.UpdateAsync(client);
        void HashClientSharedSecret(ClientSecretDto clientSecret)
        {
            if (clientSecret.Id != 0)
            {
                clientSecret.Id = 0;
            }
            else
            {
                clientSecret.Value = SHA512Utils.Encrypt(clientSecret.Value);
            }
        }
    }

    [EventHandler]
    public async Task RemoveClientAsync(RemoveClientCommand removeClientCommand)
    {
        var client = (await _clientRepository.FindAsync(removeClientCommand.ClientId))
            ?? throw new UserFriendlyException($"Client id = {removeClientCommand.ClientId} not found");
        await _clientRepository.RemoveAsync(client);
    }
    #endregion

    #region IdentityResource

    [EventHandler]
    public async Task AddIdentityResourceAsync(AddIdentityResourceCommand command)
    {
        var idrsDto = command.IdentityResource;
        var exist = await _identityResourceRepository.GetCountAsync(idrs => idrs.Name == idrsDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"IdentityResource with name {idrsDto.Name} already exists");

        var idrs = new IdentityResource(idrsDto.Name, idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        idrs.BindUserClaims(idrsDto.UserClaims);
        idrs.BindProperties(idrsDto.Properties);

        await _identityResourceRepository.AddAsync(idrs);
    }

    [EventHandler]
    public async Task UpdateIdentityResourceAsync(UpdateIdentityResourceCommand command)
    {
        var idrsDto = command.IdentityResource;
        var idrs = await _identityResourceRepository.FindAsync(idrs => idrs.Id == idrsDto.Id);
        if (idrs is null)
            throw new UserFriendlyException("The current identityResource does not exist");

        idrs.Update(idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        await _identityResourceRepository.UpdateAsync(idrs);
    }

    [EventHandler]
    public async Task RemoveIdentityResourceAsync(RemoveIdentityResourceCommand command)
    {
        var idrs = await _identityResourceRepository.FindAsync(idrs => idrs.Id == command.IdentityResource.Id);
        if (idrs == null)
            throw new UserFriendlyException("The current identityResource does not exist");

        //Todo remove check
        await _identityResourceRepository.RemoveAsync(idrs);
    }

    #endregion

    #region ApiResource

    [EventHandler]
    public async Task AddApiResourceAsync(AddApiResourceCommand command)
    {
        var apiResourceDto = command.ApiResource;
        var exist = await _apiResourceRepository.GetCountAsync(apiResource => apiResource.Name == apiResourceDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ApiResource with name {apiResourceDto.Name} already exists");

        var apiResource = new ApiResource(apiResourceDto.Name, apiResourceDto.DisplayName, apiResourceDto.Description, apiResourceDto.AllowedAccessTokenSigningAlgorithms, apiResourceDto.ShowInDiscoveryDocument, apiResourceDto.LastAccessed, apiResourceDto.NonEditable, apiResourceDto.Enabled);
        apiResource.BindUserClaims(apiResourceDto.UserClaims);
        apiResource.BindProperties(apiResourceDto.Properties);
        apiResource.BindApiScopes(apiResourceDto.ApiScopes);

        await _apiResourceRepository.AddAsync(apiResource);
    }

    [EventHandler]
    public async Task UpdateApiResourceAsync(UpdateApiResourceCommand command)
    {
        var apiResourceDto = command.ApiResource;
        var apiResource = await _apiResourceRepository.FindAsync(apiResource => apiResource.Id == apiResourceDto.Id);
        if (apiResource is null)
            throw new UserFriendlyException("The current apiResource does not exist");

        apiResource.Update(apiResourceDto.Name, apiResourceDto.DisplayName, apiResourceDto.Description, apiResourceDto.AllowedAccessTokenSigningAlgorithms, apiResourceDto.ShowInDiscoveryDocument, apiResourceDto.LastAccessed, apiResourceDto.NonEditable, apiResourceDto.Enabled);
        await _apiResourceRepository.UpdateAsync(apiResource);
    }

    [EventHandler]
    public async Task RemoveApiResourceAsync(RemoveApiResourceCommand command)
    {
        var apiResource = await _apiResourceRepository.FindAsync(apiResource => apiResource.Id == command.ApiResource.Id);
        if (apiResource == null)
            throw new UserFriendlyException("The current apiResource does not exist");

        //Todo remove check
        await _apiResourceRepository.RemoveAsync(apiResource);
    }

    #endregion

    #region ApiScope

    [EventHandler]
    public async Task AddApiScopeAsync(AddApiScopeCommand command)
    {
        var apiScopeDto = command.ApiScope;
        var exist = await _apiScopeRepository.GetCountAsync(apiScope => apiScope.Name == apiScopeDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ApiScope with name {apiScopeDto.Name} already exists");

        var apiScope = new ApiScope(apiScopeDto.Name, apiScopeDto.DisplayName, apiScopeDto.Description, apiScopeDto.Required, apiScopeDto.Emphasize, apiScopeDto.ShowInDiscoveryDocument, apiScopeDto.Enabled);
        apiScope.BindUserClaims(apiScopeDto.UserClaims);
        apiScope.BindProperties(apiScopeDto.Properties);

        await _apiScopeRepository.AddAsync(apiScope);
    }

    [EventHandler]
    public async Task UpdateApiScopeAsync(UpdateApiScopeCommand command)
    {
        var apiScopeDto = command.ApiScope;
        var apiScope = await _apiScopeRepository.FindAsync(apiScope => apiScope.Id == apiScopeDto.Id);
        if (apiScope is null)
            throw new UserFriendlyException("The current apiScope does not exist");

        apiScope.Update(apiScopeDto.Name, apiScopeDto.DisplayName, apiScopeDto.Description, apiScopeDto.Required, apiScopeDto.Emphasize, apiScopeDto.ShowInDiscoveryDocument, apiScopeDto.Enabled);
        await _apiScopeRepository.UpdateAsync(apiScope);
    }

    [EventHandler]
    public async Task RemoveApiScopeAsync(RemoveApiScopeCommand command)
    {
        var apiScope = await _apiScopeRepository.FindAsync(apiScope => apiScope.Id == command.ApiScope.Id);
        if (apiScope == null)
            throw new UserFriendlyException("The current apiScope does not exist");

        //Todo remove check
        await _apiScopeRepository.RemoveAsync(apiScope);
    }

    #endregion

    #region UserClaim

    [EventHandler]
    public async Task AddUserClaimAsync(AddUserClaimCommand command)
    {
        var userClaimDto = command.UserClaim;
        var exist = await _userClaimRepository.GetCountAsync(userClaim => userClaim.Name == userClaimDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"UserClaim with name {userClaimDto.Name} already exists");

        var userClaim = new UserClaim(userClaimDto.Name, userClaimDto.Description, userClaimDto.UserClaimType);

        await _userClaimRepository.AddAsync(userClaim);
    }

    [EventHandler]
    public async Task UpdateUserClaimAsync(UpdateUserClaimCommand command)
    {
        var userClaimDto = command.UserClaim;
        var userClaim = await _userClaimRepository.FindAsync(userClaim => userClaim.Id == userClaimDto.Id);
        if (userClaim is null)
            throw new UserFriendlyException("The current userClaim does not exist");

        userClaim.Update(userClaimDto.Name, userClaimDto.Description, userClaimDto.UserClaimType);
        await _userClaimRepository.UpdateAsync(userClaim);
    }

    [EventHandler]
    public async Task RemoveUserClaimAsync(RemoveUserClaimCommand command)
    {
        var userClaim = await _userClaimRepository.FindAsync(userClaim => userClaim.Id == command.UserClaim.Id);
        if (userClaim == null)
            throw new UserFriendlyException("The current userClaim does not exist");

        //Todo remove check
        await _userClaimRepository.RemoveAsync(userClaim);
    }

    #endregion
}
