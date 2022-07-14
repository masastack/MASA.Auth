// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

public class OperationLogCommandHandler
{
    IOperationLogRepository _operationLogRepository;
    AuthDbContext _authDbContext;

    public OperationLogCommandHandler(IOperationLogRepository operationLogRepository, AuthDbContext authDbContext)
    {
        _operationLogRepository = operationLogRepository;
        _authDbContext = authDbContext;
    }

    #region Client
    [EventHandler]
    public async Task AddClientOperationLogAsync(AddClientCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddClient, $"添加客户端：{command.AddClientDto.ClientName}");
    }

    [EventHandler]
    public async Task UpdateClientOperationLogAsync(UpdateClientCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateClient, $"编辑客户端：{command.ClientDetailDto.ClientName}");
    }

    [EventHandler(0)]
    public async Task RemoveClientOperationLogAsync(RemoveClientCommand command)
    {
        var client = await _authDbContext.Set<Client>().FirstOrDefaultAsync(client => client.Id == command.ClientId);
        if (client is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateClient, $"删除客户端：{client.ClientName}");
    }
    #endregion

    #region IdentityResource

    [EventHandler]
    public async Task AddIdentityResourceOperationLogAsync(AddIdentityResourceCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddIdentityResource, $"添加身份资源：{command.IdentityResource.Name}");
    }

    [EventHandler]
    public async Task AddStandardIdentityResourcesOperationLogAsync(AddStandardIdentityResourcesCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddStandardIdentityResources, $"添加标准身份资源");
    }

    [EventHandler]
    public async Task UpdateIdentityResourceOperationLogAsync(UpdateIdentityResourceCommand command)
    {
        var name = await GetIdentityResourceNameByIdAsync(command.IdentityResource.Id);
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateIdentityResource, $"编辑身份资源：{name}");
    }

    [EventHandler]
    public async Task RemoveIdentityResourceOperationLogAsync(RemoveIdentityResourceCommand command)
    {
        var name = await GetIdentityResourceNameByIdAsync(command.IdentityResource.Id);
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveIdentityResource, $"删除身份资源：{name}");
    }

    async Task<string?> GetIdentityResourceNameByIdAsync(int id)
    {
        var name = await _authDbContext.Set<IdentityResource>()
                                             .Where(identityResource => identityResource.Id == id)
                                             .Select(identityResource => identityResource.Name)
                                             .FirstOrDefaultAsync();
        return name;
    }

    #endregion

    #region ApiResource

    [EventHandler]
    public async Task AddApiResourceOperationLogAsync(AddApiResourceCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddApiResource, $"添加Api资源：{command.ApiResource.Name}");
    }

    [EventHandler]
    public async Task UpdateApiResourceOperationLogAsync(UpdateApiResourceCommand command)
    {
        var name = await GetApiResourceNameByIdAsync(command.ApiResource.Id);
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddApiResource, $"编辑Api资源：{name}");
    }

    [EventHandler]
    public async Task RemoveApiResourceOperationLogAsync(RemoveApiResourceCommand command)
    {
        var name = await GetApiResourceNameByIdAsync(command.ApiResource.Id);
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddApiResource, $"删除Api资源：{name}");
    }

    async Task<string?> GetApiResourceNameByIdAsync(int id)
    {
        var name = await _authDbContext.Set<ApiResource>()
                                             .Where(apiResource => apiResource.Id == id)
                                             .Select(apiResource => apiResource.Name)
                                             .FirstOrDefaultAsync();
        return name;
    }

    #endregion

    #region ApiScope

    [EventHandler]
    public async Task AddApiScopeOperationLogAsync(AddApiScopeCommand command)
    {

    }

    [EventHandler]
    public async Task UpdateApiScopeOperationLogAsync(UpdateApiScopeCommand command)
    {

    }

    [EventHandler]
    public async Task RemoveApiScopeOperationLogAsync(RemoveApiScopeCommand command)
    {

    }

    #endregion

    #region UserClaim

    [EventHandler]
    public async Task AddUserClaimOperationLogAsync(AddUserClaimCommand command)
    {

    }

    [EventHandler]
    public async Task AddStandardUserClaimsOperationLogAsync(AddStandardUserClaimsCommand command)
    {

    }

    [EventHandler]
    public async Task UpdateUserClaimOperationLogAsync(UpdateUserClaimCommand command)
    {

    }

    [EventHandler]
    public async Task RemoveUserClaimOperationLogAsync(RemoveUserClaimCommand command)
    {

    }

    #endregion

    #region CustomLogin

    [EventHandler]
    public async Task AddCustomLoginOperationLogAsync(AddCustomLoginCommand command)
    {

    }

    [EventHandler]
    public async Task UpdateCustomLoginOperationLogAsync(UpdateCustomLoginCommand command)
    {

    }

    [EventHandler]
    public async Task RemoveCustomLoginOperationLogAsync(RemoveCustomLoginCommand command)
    {

    }

    #endregion
}
