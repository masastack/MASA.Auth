using Masa.Auth.Service.Admin.Dto.Permissions;

namespace Masa.Auth.Service.Admin.Services
{
    public class PermissionService : ServiceBase
    {
        public PermissionService(IServiceCollection services) : base(services, "api/permission")
        {
            MapGet(ListAsync);
            MapGet(GetAsync);
            MapPost(CreateAsync);
            MapDelete(DeleteAsync);
        }

        private async Task CreateAsync([FromServices] IEventBus eventBus,
            [FromBody] AddPermissionCommand createDepartmentCommand)
        {
            await eventBus.PublishAsync(createDepartmentCommand);
        }

        private async Task<List<AppPermissionDto>> ListAsync([FromServices] IEventBus eventBus,
            [FromQuery] int systemId, [FromQuery] bool apiPermission)
        {
            if (apiPermission)
            {
                var apiQuery = new ApiPermissionListQuery(systemId);
                await eventBus.PublishAsync(apiQuery);
                return apiQuery.Result;
            }
            var funcQuery = new FuncPermissionListQuery(systemId);
            await eventBus.PublishAsync(funcQuery);
            return funcQuery.Result;
        }

        private async Task<PermissionDetailDto> GetAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new PermissionDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task DeleteAsync([FromServices] IEventBus eventBus,
            [FromQuery] Guid id)
        {
            await eventBus.PublishAsync(new RemovePermissionCommand(id));
        }
    }
}
