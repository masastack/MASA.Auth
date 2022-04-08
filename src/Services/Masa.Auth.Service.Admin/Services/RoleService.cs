namespace Masa.Auth.Service.Admin.Services;

public class RoleService : ServiceBase
{
    public RoleService(IServiceCollection services) : base(services, "api/role")
    {
        MapGet(GetListAsync);
        MapGet(GetSelectAsync);
        MapGet(GetDetailAsync);
        MapPost(AddAsync);
        MapPut(UpdateAsync);
        MapDelete(RemoveAsync);
    }

    private async Task<PaginationDto<RoleDto>> GetListAsync([FromServices] IEventBus eventBus,
           [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = "", [FromQuery] bool enabled = true)
    {
        var query = new RolePaginationQuery(page, pageSize, search, enabled);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus)
    {
        var query = new RoleSelectQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<RoleDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid roleId)
    {
        var query = new RoleDetailQuery(roleId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] AddRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }

    private async Task UpdateAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] UpdateRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }

    private async Task RemoveAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] RemoveRoleCommand command)
    {
        await eventBus.PublishAsync(command);
    }
}

