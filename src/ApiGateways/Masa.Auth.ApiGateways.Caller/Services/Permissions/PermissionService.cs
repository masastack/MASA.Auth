// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class PermissionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal PermissionService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/permission";
    }

    public async Task UpsertMenuPermissionAsync(MenuPermissionDetailDto dto)
    {
        await PostAsync($"CreateMenuPermission", dto);
    }

    public async Task UpsertApiPermissionAsync(ApiPermissionDetailDto dto)
    {
        await PostAsync($"CreateApiPermission", dto);
    }

    public async Task<List<SelectItemDto<PermissionTypes>>> GetTypesAsync()
    {
        return await GetAsync<List<SelectItemDto<PermissionTypes>>>($"GetTypes");
    }

    public async Task<List<AppPermissionDto>> GetApplicationPermissionsAsync(string systemId)
    {
        return await GetAsync<List<AppPermissionDto>>($"GetApplicationPermissions?systemId={systemId}");
    }

    public async Task<MenuPermissionDetailDto> GetMenuPermissionDetailAsync(Guid id)
    {
        return await GetAsync<MenuPermissionDetailDto>($"GetMenuPermission?id={id}");
    }

    public async Task<ApiPermissionDetailDto> GetApiPermissionDetailAsync(Guid id)
    {
        return await GetAsync<ApiPermissionDetailDto>($"GetApiPermission?id={id}");
    }

    public async Task<List<SelectItemDto<Guid>>> GetApiPermissionSelectAsync(string systemId)
    {
        return await GetAsync<List<SelectItemDto<Guid>>>($"GetApiPermissionSelect?systemId={systemId}");
    }

    public async Task RemoveAsync(Guid permissionId)
    {
        await DeleteAsync($"Delete?id={permissionId}");
    }

    public async Task<List<string>> GetElementPermissionsAsync(Guid userId, string appId)
    {
        return await GetAsync<List<string>>($"element-permissions?userId={userId}&appId={appId}");
    }
}
