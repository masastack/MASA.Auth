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
        var query = new GetStaffsQuery(staff.Page, staff.PageSize, staff.StaffId, staff.Enabled);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<StaffDetailDto> GetStaffDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid staffId)
    {
        var query = new StaffDetailQuery(staffId);
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
        [FromBody] AddStaffCommand createStaffCommand)
    {
        await eventBus.PublishAsync(createStaffCommand);
    }

    private async Task UpdateStaffAsync([FromServices] IEventBus eventBus,
        [FromBody] UpdateUserCommand updateUserCommand)
    {
        await eventBus.PublishAsync(updateUserCommand);
    }

    private async Task RemoveStaffAsync([FromServices] IEventBus eventBus,
        [FromBody] RemoveStaffDto staff)
    {
        var deleteCommand = new RemoveStaffCommand(staff);
        await eventBus.PublishAsync(deleteCommand);
    }
}
