namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{

    List<TeamItemDto> Teams = new List<TeamItemDto>
    {
        new TeamItemDto(Guid.NewGuid(), "Masa Stack", "", "Masa Stack Number One", "cyy", "", "cyy", DateTime.Now.AddYears(-1)),
        new TeamItemDto(Guid.NewGuid(), "Lonsid", "", "Lonsid Number One", "zjc", "", "zjc", DateTime.Now.AddYears(-10)),
    };

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<PaginationDto<TeamItemDto>> GetTeamItemsAsync(GetTeamsDto request)
    {
        var teams = Teams.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<TeamItemDto>(Teams.Count, 1, teams));
    }

    public async Task<TeamDetailDto> GetTeamDetailAsync(Guid id)
    {
        return await Task.FromResult(TeamDetailDto.Default);
    }

    public async Task<List<TeamItemDto>> SelectTeamAsync()
    {
        return await Task.FromResult(Teams);
    }

    public async Task AddTeamAsync(AddTeamDto request)
    {
        Teams.Add(new TeamItemDto(Guid.NewGuid(), request.Name, request.Avatar.Name, request.Describe, "", "", "", null));
        await Task.CompletedTask;
    }

    public async Task UpdateTeamAsync(UpdateTeamDto request)
    {
        await Task.CompletedTask;
    }
}

