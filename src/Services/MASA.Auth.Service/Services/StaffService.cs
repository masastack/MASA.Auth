namespace MASA.Auth.Service.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services)
    {
        App.MapGet(Routing.StaffList, ListAsync);
        App.MapPost(Routing.Staff, CreateAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus, [FromBody] CreateDepartmentCommand createDepartmentCommand)
    {
        await eventBus.PublishAsync(createDepartmentCommand);
    }

    private async Task<List<DepartmentItem>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new DepartmentTreeQuery(name, Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
