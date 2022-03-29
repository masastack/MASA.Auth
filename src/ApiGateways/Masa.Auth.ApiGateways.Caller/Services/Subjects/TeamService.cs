namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{
    string baseUrl = "api/staff";

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<PaginationDto<TeamDto>> ListAsync(GetTeamsDto request)
    {

    }

    public async Task<TeamDetailDto> GetAsync(Guid id)
    {
        return await Task.FromResult(new TeamDetailDto());
    }

    public async Task<List<TeamSelectDto>> TeamSelectAsync()
    {

    }

    public async Task AddAsync(AddTeamDto request)
    {

    }

    public async Task UpdateAsync(UpdateTeamDto request)
    {

    }
}

