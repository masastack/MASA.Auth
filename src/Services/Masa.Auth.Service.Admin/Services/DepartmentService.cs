namespace Masa.Auth.Service.Admin.Services;

public class DepartmentService : ServiceBase
{
    public DepartmentService(IServiceCollection services) : base(services, "api/department")
    {
        MapGet(ListAsync);
        MapPost(CreateAsync);
        MapDelete(DeleteAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus,
        [FromBody] AddDepartmentCommand createDepartmentCommand)
    {
        await eventBus.PublishAsync(createDepartmentCommand);
    }

    private async Task<List<DepartmentDto>> ListAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentTreeQuery(Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus,
        [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveDepartmentCommand(id));
    }
}

