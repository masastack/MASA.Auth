// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class UserDomainEventHandler
{
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly AuthDbContext _authDbContext;
    IStaffRepository _staffRepository;

    public UserDomainEventHandler(
        IAutoCompleteClient autoCompleteClient,
        AuthDbContext authDbContext,
        IStaffRepository staffRepository)
    {
        _autoCompleteClient = autoCompleteClient;
        _authDbContext = authDbContext;
        _staffRepository = staffRepository;
    }

    [EventHandler]
    public async Task SetUserAsync(SetUserDomainEvent userEvent)
    {
        var response = await _autoCompleteClient.SetAsync<UserSelectDto, Guid>(userEvent.Users.Select(user => new UserSelectDto(user.Id, user.Name, user.DisplayName, user.Account, user.PhoneNumber, user.Email, user.Avatar)));
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserDomainEvent userEvent)
    {
        var response = await _autoCompleteClient.DeleteAsync(userEvent.userIds);
    }

    [EventHandler(2)]
    public async Task RemoveStaffAsync(RemoveUserDomainEvent userEvent)
    {
        var staffs = await _authDbContext.Set<Staff>().Where(staff => userEvent.userIds.Contains(staff.UserId)).ToListAsync();
        await _staffRepository.RemoveRangeAsync(staffs);
    }

    [EventHandler(1)]
    public void UserRoles(QueryUserPermissionDomainEvent queryUserPermissionDomainEvent)
    {
        queryUserPermissionDomainEvent.Roles = _authDbContext.Set<UserRole>()
                .Where(ur => ur.UserId == queryUserPermissionDomainEvent.UserId && !ur.IsDeleted)
                .Select(ur => ur.RoleId).Distinct().ToList();
    }

    [EventHandler(2)]
    public void UserPermissions(QueryUserPermissionDomainEvent queryUserPermissionDomainEvent)
    {
        queryUserPermissionDomainEvent.Permissions = _authDbContext.Set<UserPermission>()
            .Where(up => up.UserId == queryUserPermissionDomainEvent.UserId && up.Effect && !up.IsDeleted)
            .Select(up => up.PermissionId).ToList();
    }

    [EventHandler(3)]
    public void UserTeamPermission(QueryUserPermissionDomainEvent queryUserPermissionDomainEvent)
    {
        var teamIdAndTypes = _authDbContext.Set<TeamStaff>()
                    .Where(t => t.Staff.UserId == queryUserPermissionDomainEvent.UserId && !t.IsDeleted)
                    .Select(t => new { t.TeamId, t.TeamMemberType });
        var teamPermissions = _authDbContext.Set<TeamPermission>()
            .Where(t => teamIdAndTypes.Any(a => a.TeamId == t.Team.Id
            && a.TeamMemberType == t.TeamMemberType) && t.Effect && !t.IsDeleted)
            .Select(t => t.PermissionId).ToList();

        queryUserPermissionDomainEvent.Permissions = queryUserPermissionDomainEvent.Permissions.Union(teamPermissions).ToList();

        var teamRoles = _authDbContext.Set<TeamRole>().Where(tr => teamIdAndTypes.Any(a => a.TeamId == tr.TeamId
            && a.TeamMemberType == tr.TeamMemberType) && !tr.IsDeleted).Select(tr => tr.RoleId).ToList();

        queryUserPermissionDomainEvent.Roles = queryUserPermissionDomainEvent.Roles.Union(teamRoles).ToList();
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
}
