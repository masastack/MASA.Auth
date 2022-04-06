namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{

    List<TeamDto> Teams = new List<TeamDto>
    {
        new TeamDto(Guid.NewGuid(), "Masa Stack", "/_content/Masa.Auth.Web.Admin.Rcl/img/subject/user.svg", "Masa Stack Number One", "cyy", "", "cyy", DateTime.Now.AddYears(-1)),
        new TeamDto(Guid.NewGuid(), "Lonsid", "/_content/Masa.Auth.Web.Admin.Rcl/img/subject/user.svg", "Lonsid Number One", "zjc", "", "zjc", DateTime.Now.AddYears(-10)),
    };

    protected override string BaseUrl { get; set; }

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/team/";
    }

    public async Task<PaginationDto<TeamDto>> GetTeamsAsync(GetTeamsDto request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var teams = Teams.Skip(skip).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<TeamDto>(Teams.Count, 1, teams));
    }

    public async Task<TeamDetailDto> GetTeamDetailAsync(Guid id)
    {
        return await Task.FromResult(new TeamDetailDto());
    }

    public async Task<List<TeamSelectDto>> TeamSelectAsync()
    {
        return await Task.FromResult(Teams.Select(t => new TeamSelectDto(t.Id, t.Name, t.Avatar)).ToList());
    }

    public async Task AddTeamAsync(AddTeamDto request)
    {
        Teams.Add(new TeamDto(Guid.NewGuid(), request.Name, request.Avatar.Name, request.Description, "", "", "", null));
        await Task.CompletedTask;
    }

    public async Task UpdateTeamAsync(UpdateTeamDto request)
    {
        await Task.CompletedTask;
    }
}

