namespace Masa.Auth.Service.Admin.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services, "api/staff")
    {
        MapGet(ListAsync);
        MapGet(PaginationAsync);
        MapPost(CreateAsync);
        MapDelete(DeleteAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus,
        [FromBody] AddStaffCommand createStaffCommand)
    {
        await eventBus.PublishAsync(createStaffCommand);
    }

    private async Task<List<StaffSelectDto>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new StaffListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<StaffDto>> PaginationAsync([FromServices] IEventBus eventBus, GetStaffsDto options)
    {
        var query = new StaffPaginationQuery(options.Page, options.PageSize, options.Name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task DeleteAsync([FromServices] IEventBus eventBus,
        [FromQuery] Guid id)
    {
        var deleteCommand = new RemoveStaffCommand(id);
        await eventBus.PublishAsync(deleteCommand);
    }
}
