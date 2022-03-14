namespace Masa.Auth.Service.Services;

public class DepartmentService : ServiceBase
{
    public DepartmentService(IServiceCollection services) : base(services)
    {
        App.MapGet(Routing.DepartmentList, ListAsync);
        App.MapPost(Routing.Department, CreateAsync);
        App.MapDelete(Routing.Department, DeleteAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus,
        [FromHeader(Name = "user-id")] Guid userId, [FromBody] CreateDepartmentCommand createDepartmentCommand)
    {
        await eventBus.PublishAsync(createDepartmentCommand);
    }

    private async Task<List<DepartmentItem>> ListAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentTreeQuery(Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus,
        [FromHeader(Name = "user-id")] Guid userId, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new DeleteDepartmentCommand(id));
    }
}

