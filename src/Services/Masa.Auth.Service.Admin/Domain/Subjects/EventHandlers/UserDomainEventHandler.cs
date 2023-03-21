// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class UserDomainEventHandler
{
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly IEventBus _eventBus;
    readonly ILogger<UserDomainEventHandler> _logger;
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IDistributedCacheClient _distributedCacheClient;

    public UserDomainEventHandler(IAutoCompleteClient autoCompleteClient, AuthDbContext authDbContext, RoleDomainService roleDomainService, IEventBus eventBus, ILogger<UserDomainEventHandler> logger, IMultilevelCacheClient multilevelCacheClient, IDistributedCacheClient distributedCacheClient)
    {
        _autoCompleteClient = autoCompleteClient;
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
        _eventBus = eventBus;
        _logger = logger;
        _multilevelCacheClient = multilevelCacheClient;
        _distributedCacheClient = distributedCacheClient;
    }

    [EventHandler(1)]
    public async Task SetAutoCompleteAsync(AddUserDomainEvent userEvent)
    {
        var user = userEvent.User.Adapt<UserSelectDto>();
        var result = await _autoCompleteClient.SetBySpecifyDocumentAsync(user);
        if (result.IsValid is false)
        {
            _logger.LogError(JsonSerializer.Serialize(result));
        }
    }

    [EventHandler(2)]
    public async Task UpdateRoleLimitAsync(AddUserDomainEvent userEvent)
    {
        var roles = userEvent.User.Roles.Select(user => user.RoleId);
        await _roleDomainService.UpdateRoleLimitAsync(roles);
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserDomainEvent userEvent)
    {
        var user = userEvent.User.Adapt<UserSelectDto>();
        await _autoCompleteClient.SetBySpecifyDocumentAsync(user);
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveUserDomainEvent userEvent)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .FirstOrDefaultAsync(staff => userEvent.User.Id == staff.UserId);
        if (staff is not null)
        {
            await _eventBus.PublishAsync(new RemoveStaffCommand(new(staff.Id)));
        }
    }

    [EventHandler(2)]
    public async Task UpdateRoleLimitAsync(RemoveUserDomainEvent userEvent)
    {
        var roles = userEvent.User.Roles.Select(user => user.RoleId);
        await _roleDomainService.UpdateRoleLimitAsync(roles);
    }

    [EventHandler(99)]
    public async Task RemoveAutoCompleteUserAsync(RemoveUserDomainEvent userEvent)
    {
        await _autoCompleteClient.DeleteAsync(userEvent.User.Id);
    }

    [EventHandler(1)]
    public async Task UpdateRoleLimitAsync(UpdateUserAuthorizationDomainEvent userEvent)
    {
        await _roleDomainService.UpdateRoleLimitAsync(userEvent.Roles);
    }

    [EventHandler(1)]
    public async Task GetPermissions(QueryUserPermissionDomainEvent userEvent)
    {
        //todo query from cache
        var user = await GetUserAsync(userEvent.UserId);
        if (user.Account == "admin")
        {
            var cachePermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
            if (cachePermissions == null || cachePermissions.Count() < 1)
            {
                userEvent.Permissions = await _authDbContext.Set<Permission>()
                        .Select(p => p.Id).ToListAsync();
            }
            else
            {
                userEvent.Permissions = cachePermissions.Select(e => e!.Id).ToList();
            }
        }
        else
        {
            var query = new PermissionsByUserQuery(userEvent.UserId, userEvent.Teams);
            await _eventBus.PublishAsync(query);
            userEvent.Permissions = query.Result;
        }
    }

    [EventHandler(2)]
    public void AuthorizedUserPermission(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var userPermissions = _authDbContext.Set<UserPermission>()
            .Where(up => up.UserId == userAuthorizedDomainEvent.UserId
            && up.PermissionId == userAuthorizedDomainEvent.PermissionId && up.Effect && !up.IsDeleted)
            .Select(up => up.PermissionId).ToList();
        //permission addition
        userAuthorizedDomainEvent.Authorized = userPermissions.Contains(userAuthorizedDomainEvent.PermissionId)
                                                || userAuthorizedDomainEvent.Authorized;
    }

    [EventHandler(3)]
    public void AuthorizedUserTeamPermission(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var teamIdAndTypes = _authDbContext.Set<TeamStaff>()
            .Where(t => t.StaffId == userAuthorizedDomainEvent.UserId && !t.IsDeleted)
            .Select(t => new { t.TeamId, t.TeamMemberType });
        var teamPermissions = _authDbContext.Set<TeamPermission>()
            .Where(t => teamIdAndTypes.Any(a => a.TeamId == t.Team.Id
            && a.TeamMemberType == t.TeamMemberType) && t.Effect && !t.IsDeleted)
            .Select(t => t.PermissionId).ToList();

        var teamRoles = _authDbContext.Set<TeamRole>().Where(tr => teamIdAndTypes.Any(a => a.TeamId == tr.TeamId
            && a.TeamMemberType == tr.TeamMemberType) && !tr.IsDeleted).Select(tr => tr.RoleId).ToList();

        userAuthorizedDomainEvent.Roles = userAuthorizedDomainEvent.Roles.Union(teamRoles).ToList();

        userAuthorizedDomainEvent.Authorized = teamPermissions.Contains(userAuthorizedDomainEvent.PermissionId)
                                                || userAuthorizedDomainEvent.Authorized;
    }

    private async Task<UserModel> GetUserAsync(Guid userId)
    {
        var userModel = await _multilevelCacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
        if (userModel == null)
        {
            var user = await _authDbContext.Set<User>()
                                       .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
            }
            userModel = user.Adapt<UserModel>();
        }
        return userModel;
    }
}
