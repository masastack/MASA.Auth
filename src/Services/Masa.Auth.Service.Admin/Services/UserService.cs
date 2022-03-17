namespace Masa.Auth.Service.Services
{
    public class UserService : ServiceBase
    {
        public UserService(IServiceCollection services) : base(services, Routing.USER_BASE_URI)
        {
            MapGet(GetUserItemsAsync);
            MapGet(GetUserDetailAsync);
            MapPost(AddUserAsync);
            MapPut(EditUserAsync);
            MapDelete(DeleteUserAsync);
        }

        private async Task<PaginationItems<UserItem>> GetUserItemsAsync([FromServices] IEventBus eventBus,
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = "",[FromQuery] bool enabled = true)
        {
            var query = new UserPaginationQuery(pageIndex, pageSize, search,enabled);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<UserDetail> GetUserDetailAsync([FromServices] IEventBus eventBus,[FromQuery] Guid userId)
        {
            var query = new UserDetailQuery(userId);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddUserAsync(
            [FromServices] IEventBus eventBus,
            [FromBody] AddUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }

        private async Task EditUserAsync(
            [FromServices] IEventBus eventBus,
            [FromBody] UpdateUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }

        private async Task DeleteUserAsync(
            [FromServices] IEventBus eventBus,
            [FromBody] RemoveUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }
    }
}
