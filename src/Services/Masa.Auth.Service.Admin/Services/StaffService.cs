namespace Masa.Auth.Service.Admin.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services, Routing.STAFF_BASE_URI)
    {
        MapGet(ListAsync);
        MapGet(PaginationAsync);
        MapPost(CreateAsync);
        MapDelete(DeleteAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus,
        [FromHeader(Name = "user-id")] Guid userId, [FromBody] AddStaffCommand createStaffCommand)
    {
        await eventBus.PublishAsync(createStaffCommand);
    }

    private async Task<List<StaffItemDto>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new StaffListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationList<StaffItemDto>> PaginationAsync([FromServices] IEventBus eventBus,
        [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string name = "")
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
