using Microsoft.AspNetCore.Mvc;

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{
    string _baseUrl = "api/team";

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<List<TeamDto>> ListAsync(string name = "")
    {
        return await GetAsync<List<TeamDto>>($"{_baseUrl}/List?name={name}");
    }

    public async Task<TeamDetailDto> GetAsync(Guid id)
    {
        return await GetAsync<TeamDetailDto>($"{_baseUrl}/Get?id={id}");
    }

    public async Task<List<TeamSelectDto>> SelectAsync(string name = "")
    {
        return await GetAsync<List<TeamSelectDto>>($"{_baseUrl}/Select?name={name}");
    }

    public async Task CreateAsync(AddTeamDto addTeamDto)
    {
        await PostAsync($"{_baseUrl}/Create", addTeamDto);
    }

    public async Task UpdateBaseInfo(UpdateTeamBaseInfoDto updateTeamBaseInfoDto)
    {
        await PostAsync($"{_baseUrl}/UpdateBaseInfo", updateTeamBaseInfoDto);
    }

    public async Task UpdateAdminPersonnel(UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await PostAsync($"{_baseUrl}/UpdateAdminPersonnel", updateTeamPersonnelDto);
    }

    public async Task UpdateMemberPersonnel(UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await PostAsync($"{_baseUrl}/UpdateMemberPersonnel", updateTeamPersonnelDto);
    }

    public async Task DeleteAsync([FromQuery] Guid id)
    {
        await DeleteAsync($"{_baseUrl}/Remove?id={id}");
    }
}
