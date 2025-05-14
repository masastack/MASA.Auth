// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

public class CommandHandler
{
    readonly IClientRepository _clientRepository;
    readonly IIdentityResourceRepository _identityResourceRepository;
    readonly IApiResourceRepository _apiResourceRepository;
    readonly IApiScopeRepository _apiScopeRepository;
    readonly IUserClaimRepository _userClaimRepository;
    readonly ICustomLoginRepository _customLoginRepository;
    readonly IUserClaimExtendRepository _userClaimExtendRepository;
    readonly IEventBus _eventBus;
    readonly SyncCache _syncCache;

    public CommandHandler(
        IClientRepository clientRepository,
        IIdentityResourceRepository identityResourceRepository,
        IApiResourceRepository apiResourceRepository,
        IApiScopeRepository apiScopeRepository,
        IUserClaimRepository userClaimRepository,
        ICustomLoginRepository customLoginRepository,
        IUserClaimExtendRepository userClaimExtendRepository,
        IEventBus eventBus,
        SyncCache syncCache,
        MasaDbContext dbContext)
    {
        _clientRepository = clientRepository;
        _identityResourceRepository = identityResourceRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
        _userClaimRepository = userClaimRepository;
        _customLoginRepository = customLoginRepository;
        _userClaimExtendRepository = userClaimExtendRepository;
        _eventBus = eventBus;
        _syncCache = syncCache;
    }

    #region Client
    [EventHandler]
    public async Task AddClientAsync(AddClientCommand addClientCommand)
    {
        var client = addClientCommand.AddClientDto.Adapt<Client>();
        client.SetClientType(addClientCommand.AddClientDto.ClientType);
        await ValidateClientIdAsync(client.ClientId);
        await _clientRepository.AddAsync(client);
    }

    [EventHandler]
    public async Task UpdateClientAsync(UpdateClientCommand updateClientCommand)
    {
        var id = updateClientCommand.ClientDetailDto.Id;
        updateClientCommand.ClientDetailDto.ClientSecrets.ForEach(secret =>
        {
            HashClientSharedSecret(secret);
        });

        var client = await _clientRepository.GetDetailAsync(id)
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CLIENT_NOT_EXIST);
        //Contrary to DDD
        updateClientCommand.ClientDetailDto.Adapt(client);

        await ValidateClientIdAsync(client.ClientId, client.Id);

        await _clientRepository.UpdateAsync(client);
        void HashClientSharedSecret(ClientSecretDto clientSecret)
        {
            if (clientSecret.Id != Guid.Empty)
            {
                clientSecret.Id = Guid.Empty;
            }
            else
            {
                clientSecret.Value = Sha512(clientSecret.Value);
            }
        }

        string Sha512(string input)
        {
            if (input.IsNullOrEmpty()) return string.Empty;

            using (var sha = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }
    }

    [EventHandler]
    public async Task RemoveClientAsync(RemoveClientCommand removeClientCommand)
    {
        var client = await _clientRepository.GetDetailAsync(removeClientCommand.ClientId)
            ?? throw new UserFriendlyException(UserFriendlyExceptionCodes.CLIENT_ID_NOT_FOUND, removeClientCommand.ClientId);
        await _clientRepository.RemoveAsync(client);
    }

    private async Task ValidateClientIdAsync(string clientId, Guid? id = null)
    {
        var client = await _clientRepository.FindAsync(c => c.ClientId == clientId && c.Id != id);
        if (client != null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.CLIENT_ID_ALREADY_EXIST, clientId);
        }
    }

    #endregion

    #region IdentityResource

    [EventHandler]
    public async Task AddIdentityResourceAsync(AddIdentityResourceCommand command)
    {
        await CheckExistApiScopeAndIdentityResouceNameAsync(command.IdentityResource.Name);

        var idrsDto = command.IdentityResource;
        var idrs = new IdentityResource(idrsDto.Name, idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        idrs.BindUserClaims(idrsDto.UserClaims);
        idrs.BindProperties(idrsDto.Properties);

        await _identityResourceRepository.AddAsync(idrs);
    }

    [EventHandler]
    public async Task AddStandardIdentityResourcesAsync(AddStandardIdentityResourcesCommand command)
    {
        await _eventBus.PublishAsync(new AddStandardUserClaimsCommand());
        await _identityResourceRepository.AddStandardIdentityResourcesAsync();
    }

    [EventHandler]
    public async Task UpdateIdentityResourceAsync(UpdateIdentityResourceCommand command)
    {
        var idrsDto = command.IdentityResource;
        var idrs = await _identityResourceRepository.GetDetailAsync(idrsDto.Id);
        if (idrs is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_RESOURCE_NOT_EXIST);

        idrs.BindUserClaims(idrsDto.UserClaims);
        idrs.BindProperties(idrsDto.Properties);
        idrs.Update(idrsDto.DisplayName, idrsDto.Description, idrsDto.Enabled, idrsDto.Required, idrsDto.Emphasize, idrsDto.ShowInDiscoveryDocument, idrsDto.NonEditable);
        await _identityResourceRepository.UpdateAsync(idrs);
    }

    [EventHandler]
    public async Task RemoveIdentityResourceAsync(RemoveIdentityResourceCommand command)
    {
        var idrs = await _identityResourceRepository.GetDetailAsync(command.IdentityResource.Id);
        if (idrs == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_RESOURCE_NOT_EXIST);

        await _identityResourceRepository.RemoveAsync(idrs);
    }

    #endregion

    #region ApiResource

    [EventHandler]
    public async Task AddApiResourceAsync(AddApiResourceCommand command)
    {
        var apiResourceDto = command.ApiResource;
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
        var apiResource = await _apiResourceRepository.GetDetailAsync(apiResourceDto.Id);
        if (apiResource is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_RESOURCE_NOT_EXIST);

        apiResource.BindUserClaims(apiResourceDto.UserClaims);
        apiResource.BindProperties(apiResourceDto.Properties);
        apiResource.BindApiScopes(apiResourceDto.ApiScopes);
        apiResource.Update(apiResourceDto.DisplayName, apiResourceDto.Description, apiResourceDto.AllowedAccessTokenSigningAlgorithms, apiResourceDto.ShowInDiscoveryDocument, apiResourceDto.LastAccessed, apiResourceDto.NonEditable, apiResourceDto.Enabled);
        await _apiResourceRepository.UpdateAsync(apiResource);
    }

    [EventHandler]
    public async Task RemoveApiResourceAsync(RemoveApiResourceCommand command)
    {
        var apiResource = await _apiResourceRepository.GetDetailAsync(command.ApiResource.Id);
        if (apiResource == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_RESOURCE_NOT_EXIST);

        await _apiResourceRepository.RemoveAsync(apiResource);
    }

    #endregion

    #region ApiScope

    [EventHandler]
    public async Task AddApiScopeAsync(AddApiScopeCommand command)
    {
        await CheckExistApiScopeAndIdentityResouceNameAsync(command.ApiScope.Name);

        var apiScopeDto = command.ApiScope;
        var apiScope = new ApiScope(apiScopeDto.Name, apiScopeDto.DisplayName, apiScopeDto.Description, apiScopeDto.Required, apiScopeDto.Emphasize, apiScopeDto.ShowInDiscoveryDocument, apiScopeDto.Enabled);
        apiScope.BindUserClaims(apiScopeDto.UserClaims);
        apiScope.BindProperties(apiScopeDto.Properties);

        await _apiScopeRepository.AddAsync(apiScope);
    }


    [EventHandler]
    public async Task UpdateApiScopeAsync(UpdateApiScopeCommand command)
    {
        var apiScopeDto = command.ApiScope;
        var apiScope = await _apiScopeRepository.GetDetailAsync(apiScopeDto.Id);
        if (apiScope is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_SCOPE_NOT_EXIST);

        apiScope.BindUserClaims(apiScopeDto.UserClaims);
        apiScope.BindProperties(apiScopeDto.Properties);
        apiScope.Update(apiScopeDto.DisplayName, apiScopeDto.Description, apiScopeDto.Required, apiScopeDto.Emphasize, apiScopeDto.ShowInDiscoveryDocument, apiScopeDto.Enabled);
        await _apiScopeRepository.UpdateAsync(apiScope);
    }

    [EventHandler]
    public async Task RemoveApiScopeAsync(RemoveApiScopeCommand command)
    {
        var apiScope = await _apiScopeRepository.GetDetailAsync(command.ApiScope.Id);
        if (apiScope == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_SCOPE_NOT_EXIST);

        await _apiScopeRepository.RemoveAsync(apiScope);
    }

    private async Task CheckExistApiScopeAndIdentityResouceNameAsync(string newName)
    {
        var apiScopeNameExistCount = await _apiScopeRepository.GetCountAsync(apiScope => apiScope.Name == newName);
        if (apiScopeNameExistCount > 0)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.API_SCOPE_NAME_EXIST);
        }

        var identityScourceNameExistCount = await _identityResourceRepository.GetCountAsync(identitySource => identitySource.Name == newName);
        if (identityScourceNameExistCount > 0)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IDENTITY_SOURCE_NAME_EXIST);
        }
    }

    #endregion

    #region UserClaim

    [EventHandler]
    public async Task AddUserClaimAsync(AddUserClaimCommand command)
    {
        var userClaimDto = command.UserClaim;
        var userClaim = new UserClaim(userClaimDto.Name, userClaimDto.Description);

        await _userClaimRepository.AddAsync(userClaim);

        // 处理UserClaimExtend数据
        if (userClaimDto.DataSourceType != DataSourceTypes.None || !string.IsNullOrEmpty(userClaimDto.DataSourceValue))
        {
            var userClaimExtend = new UserClaimExtend(userClaim.Id, userClaimDto.DataSourceType, userClaimDto.DataSourceValue);
            await _userClaimExtendRepository.AddAsync(userClaimExtend);
        }
    }

    [EventHandler]
    public async Task AddStandardUserClaimsAsync(AddStandardUserClaimsCommand command)
    {
        await _userClaimRepository.AddStandardUserClaimsAsync();
    }

    [EventHandler]
    public async Task UpdateUserClaimAsync(UpdateUserClaimCommand command)
    {
        var userClaimDto = command.UserClaim;
        var userClaim = await _userClaimRepository.FindAsync(userClaim => userClaim.Id == userClaimDto.Id);
        if (userClaim is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_CLAIM_NOT_EXIST);

        userClaim.Update(userClaimDto.Description);
        await _userClaimRepository.UpdateAsync(userClaim);

        // 处理UserClaimExtend数据
        var userClaimExtend = await _userClaimExtendRepository.FindAsync(x => x.UserClaimId == userClaimDto.Id);
        if (userClaimExtend == null)
        {
            if (userClaimDto.DataSourceType != DataSourceTypes.None || !string.IsNullOrEmpty(userClaimDto.DataSourceValue))
            {
                userClaimExtend = new UserClaimExtend(userClaimDto.Id, userClaimDto.DataSourceType, userClaimDto.DataSourceValue);
                await _userClaimExtendRepository.AddAsync(userClaimExtend);
            }
        }
        else
        {
            if (userClaimDto.DataSourceType == DataSourceTypes.None && string.IsNullOrEmpty(userClaimDto.DataSourceValue))
            {
                await _userClaimExtendRepository.RemoveAsync(userClaimExtend);
            }
            else
            {
                userClaimExtend.UpdateDataSource(userClaimDto.DataSourceType, userClaimDto.DataSourceValue);
                await _userClaimExtendRepository.UpdateAsync(userClaimExtend);
            }
        }
    }

    [EventHandler]
    public async Task RemoveUserClaimAsync(RemoveUserClaimCommand command)
    {
        var userClaim = await _userClaimRepository.FindAsync(userClaim => userClaim.Id == command.UserClaim.Id);
        if (userClaim == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_CLAIM_NOT_EXIST);

        //Todo remove check
        await _userClaimRepository.RemoveAsync(userClaim);

        // 删除关联的UserClaimExtend数据
        var userClaimExtend = await _userClaimExtendRepository.FindAsync(x => x.UserClaimId == command.UserClaim.Id);
        if (userClaimExtend != null)
        {
            await _userClaimExtendRepository.RemoveAsync(userClaimExtend);
        }
    }

    #endregion

    #region CustomLogin

    [EventHandler]
    public async Task AddCustomLoginAsync(AddCustomLoginCommand command)
    {
        var customLoginDto = command.CustomLogin;
        if (customLoginDto.Enabled is true)
        {
            var exist = await _customLoginRepository.GetCountAsync(customLogin => customLogin.ClientId == customLoginDto.ClientId && customLogin.Enabled == true) > 0;
            if (exist)
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CUSTOM_LOGIN_ENABLE_MULTIPLE);
        }

        var customLogin = new CustomLogin(customLoginDto.Name, customLoginDto.Title, customLoginDto.ClientId, customLoginDto.Enabled);
        customLogin.BindRegisterFields(customLoginDto.RegisterFields);
        customLogin.BindThirdPartyIdps(customLoginDto.ThirdPartyIdps);
        await _customLoginRepository.AddAsync(customLogin);
    }

    [EventHandler]
    public async Task UpdateCustomLoginAsync(UpdateCustomLoginCommand command)
    {
        var customLoginDto = command.CustomLogin;
        var customLogin = await _customLoginRepository.GetDetailAsync(customLoginDto.Id);
        if (customLogin is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CUSTOM_LOGIN_NOT_EXIST);

        if (customLoginDto.Enabled is true)
        {
            var exist = await _customLoginRepository.GetCountAsync(cl => cl.Id != customLoginDto.Id && cl.ClientId == customLogin.ClientId && cl.Enabled == true) > 0;
            if (exist)
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CUSTOM_LOGIN_ENABLE_MULTIPLE);
        }
        customLogin.Update(customLoginDto.Name, customLoginDto.Title, customLoginDto.Enabled);
        customLogin.BindRegisterFields(customLoginDto.RegisterFields);
        customLogin.BindThirdPartyIdps(customLoginDto.ThirdPartyIdps);
        await _customLoginRepository.UpdateAsync(customLogin);
    }

    [EventHandler]
    public async Task RemoveCustomLoginAsync(RemoveCustomLoginCommand command)
    {
        var customLogin = await _customLoginRepository.GetDetailAsync(command.CustomLogin.Id);
        if (customLogin == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CUSTOM_LOGIN_NOT_EXIST);

        await _customLoginRepository.RemoveAsync(customLogin);
    }

    #endregion

    [EventHandler]
    public async Task SyncOidcAsync(SyncOidcCommand command)
    {
        await _syncCache.ResetAsync();
    }
}
