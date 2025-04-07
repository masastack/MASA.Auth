// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class PermissionService : ServiceBase
{
    public PermissionService() : base("api/permission")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapGet(GetApplicationPermissionsAsync);
        MapGet(GetTypesAsync);
        MapGet(GetApiPermissionSelectAsync);
        MapGet(GetMenuPermissionAsync);
        MapGet(GetApiPermissionAsync);
        MapPost(CreateMenuPermissionAsync).RequireAuthorization();
        MapPost(CreateApiPermissionAsync).RequireAuthorization();
        MapDelete(DeleteAsync).RequireAuthorization();
        MapGet(GetMenusAsync, "menus");
        MapGet(AuthorizedAsync);
        MapGet(GetElementPermissionsAsync, "element-permissions");
        MapGet(GetAppPermissionsAsync, "app-permissions");
        MapPut(AddFavoriteMenuAsync).RequireAuthorization();
        MapPut(RemoveFavoriteMenuAsync).RequireAuthorization();
        MapGet(GetFavoriteMenuListAsync, "menu-favorite-list");
        MapGet(GetPermissionsByRoleAsync);
        MapPost(GetPermissionsByTeamAsync);
        MapPost(GetPermissionsByTeamWithUserAsync);
        MapPost(SyncRedisAsync);
        MapGet(GetAppGlobalNavVisibleAsync);
        MapGet(GetAppGlobalNavVisibleListAsync);
        MapPost(SaveAppGlobalNavVisibleAsync).RequireAuthorization();
        MapGet(GetI18NDisplayNameAsync, "i18n-display-name");
        MapPost(SaveI18NDisplayNameAsync, "i18n-display-name").RequireAuthorization();
    }

    private async Task<List<SelectItemDto<int>>> GetTypesAsync(IEventBus eventBus)
    {
        var query = new PermissionTypesQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<SelectItemDto<Guid>>> GetApiPermissionSelectAsync(IEventBus eventBus, [FromQuery] string systemId)
    {
        var query = new ApiPermissionSelectQuery(systemId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<MenuPermissionDetailDto?> CreateMenuPermissionAsync(IEventBus eventBus,
        [FromBody] MenuPermissionDetailDto dto)
    {
        var command = new UpsertPermissionCommand(dto);
        await eventBus.PublishAsync(command);
        return command.PermissionDetail as MenuPermissionDetailDto;
    }

    private async Task<ApiPermissionDetailDto?> CreateApiPermissionAsync(IEventBus eventBus,
        [FromBody] ApiPermissionDetailDto apiPermissionDetailDto)
    {
        var command = new UpsertPermissionCommand(apiPermissionDetailDto);
        await eventBus.PublishAsync(command);
        return command.PermissionDetail as ApiPermissionDetailDto;
    }

    private async Task<List<AppPermissionDto>> GetApplicationPermissionsAsync(IEventBus eventBus, [FromQuery] string systemId)
    {
        var funcQuery = new ApplicationPermissionsQuery(systemId);
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

    private async Task<List<MenuModel>> GetMenusAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] Guid userId)
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

    private async Task<List<string>> GetAppPermissionsAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] Guid userId)
    {
        var query = new AppPermissionCodeListQuery(appId, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<bool> AuthorizedAsync(IEventBus eventBus, [FromQuery] string appId, [FromQuery] string code, [FromQuery] Guid userId)
    {
        var query = new AppPermissionAuthorizedQuery(appId, code, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddFavoriteMenuAsync(IEventBus eventBus, [FromQuery] Guid permissionId, [FromQuery] Guid userId)
    {
        var command = new FavoriteMenuCommand(permissionId, userId, true);
        await eventBus.PublishAsync(command);
    }

    private async Task RemoveFavoriteMenuAsync(IEventBus eventBus, [FromQuery] Guid permissionId, [FromQuery] Guid userId)
    {
        var command = new FavoriteMenuCommand(permissionId, userId, false);
        await eventBus.PublishAsync(command);
    }

    private async Task<List<SelectItemDto<Guid>>> GetFavoriteMenuListAsync(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new FavoriteMenuListQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<Guid>> GetPermissionsByRoleAsync([FromServices] IEventBus eventBus, [FromQuery] string ids)
    {
        if (string.IsNullOrEmpty(ids)) return new();
        var roles = ids.Split(',').Select(id => Guid.Parse(id)).ToList();
        var query = new PermissionsByRoleQuery(roles);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<Guid>> GetPermissionsByTeamAsync([FromServices] IEventBus eventBus, List<TeamSampleDto> teams)
    {
        var query = new PermissionsByTeamQuery(teams);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<Guid>> GetPermissionsByTeamWithUserAsync([FromServices] IEventBus eventBus, GetPermissionsByTeamWithUserDto dto)
    {
        var query = new PermissionsByTeamWithUserQuery(dto);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    public async Task SyncRedisAsync([FromServices] IEventBus eventBus)
    {
        var command = new SyncPermissionRedisCommand();
        await eventBus.PublishAsync(command);
    }

    private async Task<AppGlobalNavVisibleDto> GetAppGlobalNavVisibleAsync(IEventBus eventBus, [FromQuery] string appId)
    {
        var query = new AppGlobalNavVisibleQuery(appId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<AppGlobalNavVisibleDto>> GetAppGlobalNavVisibleListAsync(IEventBus eventBus, [FromQuery] string appIds)
    {
        var query = new AppGlobalNavVisibleListQuery(appIds.Split(','));
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task SaveAppGlobalNavVisibleAsync(IEventBus eventBus,
        [FromBody] AppGlobalNavVisibleDto dto)
    {
        var command = new SaveAppGlobalNavVisibleCommand(dto);
        await eventBus.PublishAsync(command);
    }

    private async Task<List<PermissionI18NDisplayNameDto>> GetI18NDisplayNameAsync(IEventBus eventBus, string cultureName, string name)
    {
        var query = new GetI18NDisplayNameQuery(cultureName.Split(','), name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task SaveI18NDisplayNameAsync(IEventBus eventBus, SaveI18NDisplayNamesInput input)
    {
        var command = new SaveI18NDisplayNameCommand(input);
        await eventBus.PublishAsync(command);
    }
}
