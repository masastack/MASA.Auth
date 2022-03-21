namespace Masa.Auth.Service.Admin.Services;

public class RoleService : ServiceBase
{
    public RoleService(IServiceCollection services) : base(services, "api/role")
    {
        MapGet(GetRolesAsync);
        MapGet(GetRoleSelectAsync);
        MapGet(GetRoleDetailAsync);
        MapPost(AddRoleAsync);
        MapPut(EditRoleAsync);
        MapDelete(DeleteRoleAsync);
    }

    private async Task<PaginationDto<RoleDto>> GetRolesAsync([FromServices] IEventBus eventBus,
           [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = "", [FromQuery] bool enabled = true)
    {
        var query = new RolePaginationQuery(pageIndex, pageSize, search, enabled);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetRoleSelectAsync([FromServices] IEventBus eventBus)
    {
        var query = new RoleSelectQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<RoleDetailDto> GetRoleDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid roleId)
    {
        var query = new RoleDetailQuery(roleId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddRoleAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] AddRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }

    private async Task EditRoleAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] UpdateRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }

    private async Task DeleteRoleAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] RemoveRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }
}

