namespace Masa.Auth.Service.Services;

public class StaffService : ServiceBase
{
    public StaffService(IServiceCollection services) : base(services)
    {
        App.MapGet(Routing.StaffList, ListAsync);
        App.MapPost(Routing.Staff, CreateAsync);
    }

    private async Task CreateAsync([FromServices] IEventBus eventBus, [FromBody] CreateStaffCommand createStaffCommand)
    {

    }

    private async Task<List<DepartmentItem>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        throw new NotImplementedException();
    }
}
