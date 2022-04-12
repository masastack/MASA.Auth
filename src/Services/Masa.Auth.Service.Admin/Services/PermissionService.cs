namespace Masa.Auth.Service.Admin.Services
{
    public class PermissionService : ServiceBase
    {
        public PermissionService(IServiceCollection services) : base(services, "api/permission")
        {
            MapGet(ListAsync);
            MapGet(GetTypesAsync);
            MapGet(GetApiPermissionSelectAsync);
            MapGet(GetAsync);
            MapPost(CreateAsync);
            MapDelete(DeleteAsync);
        }

        private async Task<List<SelectItemDto<int>>> GetTypesAsync(IEventBus eventBus)
        {
            var query = new PermissionTypesQuery();
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<SelectItemDto<Guid>>> GetApiPermissionSelectAsync(IEventBus eventBus, [FromQuery] string name)
        {
            var query = new ApiPermissionSelectQuery(name);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task CreateAsync(IEventBus eventBus,
            [FromBody] AddPermissionCommand createDepartmentCommand)
        {
            await eventBus.PublishAsync(createDepartmentCommand);
        }

        private async Task<List<AppPermissionDto>> ListAsync(IEventBus eventBus,
            [FromQuery] int systemId, [FromQuery] bool apiPermission)
        {
            if (apiPermission)
            {
                var apiQuery = new ApiPermissionListQuery(systemId);
                await eventBus.PublishAsync(apiQuery);
                return apiQuery.Result;
            }
            var funcQuery = new MenuPermissionListQuery(systemId);
            await eventBus.PublishAsync(funcQuery);
            return funcQuery.Result;
        }

        private async Task<MenuPermissionDetailDto> GetAsync(IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new MenuPermissionDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task DeleteAsync(IEventBus eventBus, [FromQuery] Guid id)
        {
            await eventBus.PublishAsync(new RemovePermissionCommand(id));
        }
    }
}
