namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    readonly IPmClient _pmClient;

    public QueryHandler(IPmClient pmClient)
    {
        _pmClient = pmClient;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        var projects = await _pmClient.ProjectService.GetProjectListAsync("development");
        query.Result = projects.Select(p => new ProjectDto
        {
            Name = p.Name,
            Id = p.Id,
            Identity = p.Identity,
            Apps = p.Apps.Select(a => new AppDto
            {
                Name = a.Name,
                Id = a.Id,
                Identity = a.Identity,
                ProjectId = a.ProjectId
            }).ToList()
        }).ToList();
        await Task.CompletedTask;
    }
}
