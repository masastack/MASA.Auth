namespace Masa.Auth.ApiGateways.Caller.Services.Projects;

public class ProjectService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ProjectService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/project";
    }

    public async Task<List<ProjectDto>> GetListAsync()
    {
        return await GetAsync<List<ProjectDto>>($"GetList");
    }
}
