namespace Masa.Auth.Service.Admin.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services, "api/staff")
    {
        MapGet(GetStaffsAsync);
        MapGet(GetStaffDetailAsync);
        MapGet(GetStaffSelectAsync);
        MapPut(AddStaffAsync);
        MapPost(UpdateStaffAsync);
        MapDelete(RemoveStaffAsync);
    }

    private async Task<PaginationDto<StaffDto>> GetStaffsAsync([FromServices] IEventBus eventBus, GetStaffsDto staff)
    {
        var query = new GetStaffsQuery(staff.Page, staff.PageSize, staff.Search, staff.Enabled);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<StaffDetailDto> GetStaffDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<StaffSelectDto>> GetStaffSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new StaffSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddStaffAsync([FromServices] IEventBus eventBus,
        [FromBody] AddStaffDto staff)
    {
        await eventBus.PublishAsync(new AddStaffCommand(staff));
    }

    private async Task UpdateStaffAsync([FromServices] IEventBus eventBus,
        [FromBody] UpdateStaffDto staff)
    {
        await eventBus.PublishAsync(new UpdateStaffCommand(staff));
    }

    private async Task RemoveStaffAsync([FromServices] IEventBus eventBus,
        [FromBody] RemoveStaffDto staff)
    {
        var deleteCommand = new RemoveStaffCommand(staff);
        await eventBus.PublishAsync(deleteCommand);
    }
}
