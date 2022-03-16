namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{

    List<TeamItemResponse> Teams = new List<TeamItemResponse>
    {
        new TeamItemResponse(Guid.NewGuid(), "Masa Stack", "", "Masa Stack Number One", "cyy", "", "cyy", DateTime.Now.AddYears(-1)),
        new TeamItemResponse(Guid.NewGuid(), "Lonsid", "", "Lonsid Number One", "zjc", "", "zjc", DateTime.Now.AddYears(-10)),
    };

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<PaginationItemsResponse<TeamItemResponse>> GetTeamItemsAsync(GetTeamItemsRequest request)
    {
        var teams = Teams.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
        return await Task.FromResult(new PaginationItemsResponse<TeamItemResponse>(Teams.Count, 1, teams));
    }

    public async Task<TeamDetailResponse> GetTeamDetailAsync(Guid id)
    {
        return await Task.FromResult(TeamDetailResponse.Default);
    }

    public async Task<List<TeamItemResponse>> SelectTeamAsync()
    {
        return await Task.FromResult(Teams);
    }

    public async Task AddTeamAsync(AddTeamRequest request)
    {
        Teams.Add(new TeamItemResponse(Guid.NewGuid(), request.Name, request.Avatar.Name, request.Describe, "", "", "", null));
        await Task.CompletedTask;
    }

    public async Task UpdateTeamAsync(UpdateTeamRequest request)
    {
        await Task.CompletedTask;
    }
}

