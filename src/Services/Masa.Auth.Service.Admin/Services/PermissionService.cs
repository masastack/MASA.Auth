// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class PermissionService : ServiceBase
{
    public PermissionService(IServiceCollection services) : base(services, "api/permission")
    {
        MapGet(GetApplicationPermissionsAsync);
        MapGet(GetChildMenuPermissionsAsync);
        MapGet(GetTypesAsync);
        MapGet(GetApiPermissionSelectAsync);
        MapGet(GetMenuPermissionAsync);
        MapGet(GetApiPermissionAsync);
        MapPost(CreateMenuPermissionAsync);
        MapPost(CreateApiPermissionAsync);
        MapDelete(DeleteAsync);
        MapGet(GetMenusAsync, "menus");
        MapGet(AuthorizedAsync);
        MapGet(GetElementPermissionsAsync, "element-permissions");
        MapPut(CollectAsync);
        MapPut(UnCollectAsync);
        MapGet(CollectListAsync, "collect-list");
    }

    private async Task<List<SelectItemDto<int>>> GetTypesAsync(IEventBus eventBus)
    {
        var query = new PermissionTypesQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<SelectItemDto<Guid>>> GetApiPermissionSelectAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new ApiPermissionSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task CreateMenuPermissionAsync(IEventBus eventBus,
        [FromBody] MenuPermissionDetailDto menuPermissionDetailDto)
    {
        await eventBus.PublishAsync(new AddPermissionCommand(menuPermissionDetailDto)
        {
            Enabled = menuPermissionDetailDto.Enabled,
            ParentId = menuPermissionDetailDto.ParentId,
            ApiPermissions = menuPermissionDetailDto.ApiPermissions
        });
    }

    private async Task CreateApiPermissionAsync(IEventBus eventBus,
        [FromBody] ApiPermissionDetailDto apiPermissionDetailDto)
    {
        await eventBus.PublishAsync(new AddPermissionCommand(apiPermissionDetailDto));
    }

    private async Task<List<AppPermissionDto>> GetApplicationPermissionsAsync(IEventBus eventBus, [FromQuery] string systemId)
    {
        var funcQuery = new ApplicationPermissionsQuery(systemId);
        await eventBus.PublishAsync(funcQuery);
        return funcQuery.Result;
    }

    private async Task<List<PermissionDto>> GetChildMenuPermissionsAsync(IEventBus eventBus, [FromQuery] Guid permissionId)
    {
        var funcQuery = new ChildMenuPermissionsQuery(permissionId);
        await eventBus.PublishAsync(funcQuery);
        return funcQuery.Result;
    }

    private async Task<MenuPermissionDetailDto> GetMenuPermissionAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new MenuPermissionDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ApiPermissionDetailDto> GetApiPermissionAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ApiPermissionDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task DeleteAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemovePermissionCommand(id));
    }

    private async Task<List<MenuDto>> GetMenusAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] Guid userId)
    {
        var query = new AppMenuListQuery(appId, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<string>> GetElementPermissionsAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] Guid userId)
    {
        var query = new AppElementPermissionCodeListQuery(appId, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<bool> AuthorizedAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] string code, [FromQuery] Guid userId)
    {
        var query = new AppPermissionAuthorizedQuery(appId, code, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task CollectAsync(IEventBus eventBus, [FromQuery] Guid permissionId, [FromQuery] Guid userId)
    {
        var command = new CollectMenuCommand(permissionId, userId, true);
        await eventBus.PublishAsync(command);
    }

    private async Task UnCollectAsync(IEventBus eventBus, [FromQuery] Guid permissionId, [FromQuery] Guid userId)
    {
        var command = new CollectMenuCommand(permissionId, userId, false);
        await eventBus.PublishAsync(command);
    }

    private async Task<List<SelectItemDto<Guid>>> CollectListAsync(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new CollectMenuListQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
