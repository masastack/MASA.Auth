namespace Masa.Auth.Service.Admin.Services;

public class DepartmentService : ServiceBase
{
    public DepartmentService(IServiceCollection services) : base(services, "api/department")
    {
        MapGet(GetAsync);
        MapGet(ListAsync);
        MapGet(CountAsync);
        MapPost(SaveAsync);
        MapDelete(RemoveAsync);
    }

    private async Task SaveAsync([FromServices] IEventBus eventBus,
        [FromBody] UpsertDepartmentDto upsertDepartmentDto)
    {
        await eventBus.PublishAsync(new AddDepartmentCommand(upsertDepartmentDto));
    }

    private async Task<DepartmentDetailDto> GetAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new DepartmentDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<DepartmentDto>> ListAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentTreeQuery(Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync([FromServices] IEventBus eventBus,
        [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveDepartmentCommand(id));
    }

    private async Task<DepartmentChildrenCountDto> CountAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentCountQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}

