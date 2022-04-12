namespace Masa.Auth.Service.Admin.Services
{
    public class PermissionService : ServiceBase
    {
        public PermissionService(IServiceCollection services) : base(services, "api/permission")
        {
            MapGet(GetApplicationPermissionsAsync);
            MapGet(GetChildMenuPermissionsAsync);
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

        private async Task<List<AppPermissionDto>> GetApplicationPermissionsAsync(IEventBus eventBus, [FromQuery] int systemId)
        {
            var funcQuery = new ApplicationPermissionsQuery(systemId);
            await eventBus.PublishAsync(funcQuery);
            return funcQuery.Result;
        }

        private async Task<List<PermissionDto>> GetChildMenuPermissionsAsync(IEventBus eventBus, [FromQuery] Guid permissionId)
        {
            var funcQuery = new ChildMenuPermissionsQuery(permissionId);
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
