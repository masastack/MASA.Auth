namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    readonly IPmClient _pmClient;
    readonly IAppNavigationTagRepository _appNavigationTagRepository;

    public QueryHandler(IPmClient pmClient, IAppNavigationTagRepository appNavigationTagRepository)
    {
        _pmClient = pmClient;
        _appNavigationTagRepository = appNavigationTagRepository;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        var projects = await _pmClient.ProjectService.GetProjectListAsync("development");
        if (projects.Any())
        {
            var appTags = await _appNavigationTagRepository.GetListAsync();
            query.Result = projects.Select(p => new ProjectDto
            {
                Name = p.Name,
                Id = p.Id,
                Identity = p.Identity,
                Apps = p.Apps.Select(a => new AppDto
                {
                    Name = a.Name,
                    Id = a.Id,
                    Tag = appTags.FirstOrDefault(at => at.AppIdentity == a.Identity)?.Tag ?? "",
                    Identity = a.Identity,
                    ProjectId = a.ProjectId
                }).ToList()
            }).ToList();
        }
        await Task.CompletedTask;
    }

    [EventHandler]
    public async Task AppTagsAsync(AppTagsQuery query)
    {
        var tags = new List<string>() { "Tag1", "Tag2", "Tag3" };
        query.Result = tags;
        await Task.CompletedTask;
    }
}
