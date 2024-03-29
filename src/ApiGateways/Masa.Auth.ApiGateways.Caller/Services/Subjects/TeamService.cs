﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal TeamService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/team/";
    }

    public async Task<List<TeamDto>> ListAsync(string name = "")
    {
        return await GetAsync<List<TeamDto>>($"List?name={name}");
    }

    public async Task<TeamDetailDto> GetAsync(Guid id)
    {
        return await GetAsync<TeamDetailDto>($"Get?id={id}");
    }

    public async Task<List<TeamSelectDto>> SelectAsync(string name = "")
    {
        return await GetAsync<List<TeamSelectDto>>($"Select?name={name}");
    }

    public async Task<List<TeamRoleSelectDto>> GetTeamRoleSelectAsync(string name = "", Guid userId = default)
    {
        return await SendAsync<object, List<TeamRoleSelectDto>>(nameof(GetTeamRoleSelectAsync), new { name, userId });
    }

    public async Task CreateAsync(AddTeamDto addTeamDto)
    {
        await PostAsync($"Create", addTeamDto);
    }

    public async Task Update(UpdateTeamDto updateTeamDto)
    {
        await PostAsync($"Update", updateTeamDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"Remove?id={id}");
    }
}
