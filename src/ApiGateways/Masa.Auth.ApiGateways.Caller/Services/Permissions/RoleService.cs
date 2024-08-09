// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class RoleService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal RoleService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/role/";
    }

    public async Task<PaginationDto<RoleDto>> GetListAsync(GetRolesDto request)
    {
        return await SendAsync<GetRolesDto, PaginationDto<RoleDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<RoleSelectDto>> GetTopRoleSelectAsync(Guid roleId)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetTopRoleSelectAsync), new { roleId });
    }

    public async Task<List<RoleSelectDto>> GetSelectForUserAsync(Guid userId = default)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForUserAsync), new { userId });
    }


    public async Task<List<RoleSelectDto>> GetSelectForRoleAsync(Guid roleId = default)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForRoleAsync), new { roleId });
    }

    public async Task<List<RoleSelectDto>> GetSelectForTeamAsync(Guid teamId = default, TeamMemberTypes teamMemberType = TeamMemberTypes.Member)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForTeamAsync), new { teamId, teamMemberType });
    }

    public async Task<RoleDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, RoleDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task<RoleOwnerDto> GetRoleOwnerAsync(Guid id)
    {
        return await SendAsync<object, RoleOwnerDto>(nameof(GetRoleOwnerAsync), new { id });
    }

    public async Task AddAsync(AddRoleDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateRoleDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveRoleDto(id));
    }

    public async Task AddUserAsync(Guid id, List<Guid> userIds)
    {
        await PostAsync($"{id}/user", userIds);
    }

    public async Task RemoveUserAsync(Guid id, List<Guid> userIds)
    {
        await DeleteAsync($"{id}/user", userIds);
    }

    public async Task<PaginationDto<UserSelectModel>> GetUsersAsync(Guid id, PaginatedOptionsDto options)
    {
        return await GetAsync<object, PaginationDto<UserSelectModel>>($"{id}/user", options);
    }
}

