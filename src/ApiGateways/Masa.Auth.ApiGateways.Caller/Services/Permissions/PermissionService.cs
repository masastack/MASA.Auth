// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class PermissionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal PermissionService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/permission";
    }

    public async Task<MenuPermissionDetailDto> UpsertMenuPermissionAsync(MenuPermissionDetailDto dto)
    {
        var result = await PostAsync<MenuPermissionDetailDto, MenuPermissionDetailDto>($"CreateMenuPermission", dto);
        return result;
    }

    public async Task<ApiPermissionDetailDto> UpsertApiPermissionAsync(ApiPermissionDetailDto dto)
    {
        var result = await PostAsync<ApiPermissionDetailDto, ApiPermissionDetailDto>($"CreateApiPermission", dto);
        return result;
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

    public async Task<List<Guid>> GetPermissionsByRoleAsync(List<Guid> roles)
    {
        return await SendAsync<object, List<Guid>>(nameof(GetPermissionsByRoleAsync), new { ids = string.Join(',', roles) });
    }

    public async Task<List<Guid>> GetPermissionsByTeamAsync(List<TeamSampleDto> teams)
    {
        return await PostAsync<List<TeamSampleDto>, List<Guid>>(nameof(GetPermissionsByRoleAsync), teams);
    }

    public async Task<List<Guid>> GetPermissionsByTeamWithUserAsync(GetPermissionsByTeamWithUserDto dto)
    {
        return await PostAsync<GetPermissionsByTeamWithUserDto, List<Guid>>(nameof(GetPermissionsByTeamWithUserAsync), dto);
    }

    public async Task<AppGlobalNavVisibleDto> GetAppGlobalNavVisibleAsync(string appId)
    {
        return await GetAsync<AppGlobalNavVisibleDto>($"GetAppGlobalNavVisible?appId={appId}");
    }

    public async Task<List<AppGlobalNavVisibleDto>> GetAppGlobalNavVisibleListAsync(string appIds)
    {
        return await GetAsync<List<AppGlobalNavVisibleDto>>($"GetAppGlobalNavVisibleList?appIds={appIds}");
    }

    public async Task SaveAppGlobalNavVisibleAsync(AppGlobalNavVisibleDto visibleDto)
    {
        await PostAsync("SaveAppGlobalNavVisible", visibleDto);
    }
}
