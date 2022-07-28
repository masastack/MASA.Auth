// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
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

    public async Task<List<TeamRoleSelectDto>> GetTeamRoleSelectAsync(string name = "")
    {
        return await SendAsync<object, List<TeamRoleSelectDto>>(nameof(GetTeamRoleSelectAsync), new { name });
    }

    public async Task CreateAsync(AddTeamDto addTeamDto)
    {
        await PostAsync($"Create", addTeamDto);
    }

    public async Task UpdateBasicInfo(UpdateTeamBasicInfoDto updateTeamBasicInfoDto)
    {
        await PostAsync($"UpdateBasicInfo", updateTeamBasicInfoDto);
    }

    public async Task UpdateAdminPersonnel(UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await PostAsync($"UpdateAdminPersonnel", updateTeamPersonnelDto);
    }

    public async Task UpdateMemberPersonnel(UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await PostAsync($"UpdateMemberPersonnel", updateTeamPersonnelDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"Remove?id={id}");
    }
}
