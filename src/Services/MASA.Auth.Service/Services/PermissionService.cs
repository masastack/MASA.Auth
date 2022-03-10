namespace Masa.Auth.Service.Services
{
    public class PermissionService : ServiceBase
    {
        public PermissionService(IServiceCollection services) : base(services)
        {
            App.MapGet(Routing.PermissionList, ListAsync);
            App.MapGet(Routing.PermissionDetail, GetAsync);
            App.MapPost(Routing.Permission, CreateAsync);
            App.MapDelete(Routing.Permission, DeleteAsync);
        }

        private async Task CreateAsync([FromServices] IEventBus eventBus,
            [FromBody] CreatePermissionCommand createDepartmentCommand)
        {
            await eventBus.PublishAsync(createDepartmentCommand);
        }

        private async Task<List<AppPermissionItem>> ListAsync([FromServices] IEventBus eventBus,
            [FromQuery] int systemId, [FromQuery] bool apiPermission)
        {
            if (apiPermission)
            {
                var apiQuery = new ApiPermissionsQuery(systemId);
                await eventBus.PublishAsync(apiQuery);
                return apiQuery.Result;
            }
            var funcQuery = new FuncPermissionsQuery(systemId);
            await eventBus.PublishAsync(funcQuery);
            return funcQuery.Result;
        }

        private async Task<PermissionDetail> GetAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new PermissionDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task DeleteAsync([FromServices] IEventBus eventBus,
            [FromQuery] Guid id)
        {
            await eventBus.PublishAsync(new DeletePermissionCommand(id));
        }
    }
}
