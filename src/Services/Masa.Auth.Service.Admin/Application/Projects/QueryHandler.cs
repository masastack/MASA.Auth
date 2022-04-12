namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    [EventHandler]
    private async Task GetProjectListAsync(ProjectListQuery query)
    {
        query.Result = new List<ProjectDto>
        {
            new ProjectDto
            {
                Name = "Project1",
                Id =1,
                Identity="11111111",
                Apps = new List<AppDto>
                {
                    new AppDto{ Id=1,Name="App1",Identity="pro-1-app-1",ProjectId=1},
                    new AppDto{ Id=2,Name="App2",Identity="pro-1-app-2",ProjectId=1},
                }
            },
            new ProjectDto
            {
                Name = "Project2",
                Id =2,
                Identity="2222222",
                Apps = new List<AppDto>
                {
                    new AppDto{ Id=3,Name="App1",Identity="pro-2-app-3",ProjectId=2},
                    new AppDto{ Id=4,Name="App2",Identity="pro-2-app-4",ProjectId=2},
                }
            }
        };
        await Task.CompletedTask;
    }
}
