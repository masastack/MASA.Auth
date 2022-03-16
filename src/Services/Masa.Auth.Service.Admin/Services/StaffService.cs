namespace Masa.Auth.Service.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services)
    {
        App.MapGet(Routing.StaffList, ListAsync);
        App.MapGet(Routing.StaffPagination, PaginationAsync);
        App.MapPost(Routing.Staff, CreateAsync);
        App.MapDelete(Routing.Staff, DeleteAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus,
        [FromHeader(Name = "user-id")] Guid userId, [FromBody] AddStaffCommand createStaffCommand)
    {
        await eventBus.PublishAsync(createStaffCommand);
    }

    private async Task<List<StaffItem>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new StaffListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationItems<StaffItem>> PaginationAsync([FromServices] IEventBus eventBus,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string name = "")
    {
        var query = new StaffPaginationQuery(pageIndex, pageSize, name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus,
        [FromHeader(Name = "user-id")] Guid userId, [FromQuery] Guid id)
    {
        var deleteCommand = new RemoveStaffCommand(id);
        await eventBus.PublishAsync(deleteCommand);
    }
}
