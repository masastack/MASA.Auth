namespace Masa.Auth.Service.Services
{
    public class UserService : ServiceBase
    {
        public UserService(IServiceCollection services) : base(services)
        {
            App.MapGet(Routing.UserList, PaginationAsync);
            App.MapPost(Routing.OperateUser, AddUserAsync);
            App.MapPut(Routing.OperateUser, EditUserAsync);
            App.MapDelete(Routing.OperateUser, DeleteUserAsync);
        }

        private async Task<PaginationList<UserItem>> PaginationAsync([FromServices] IEventBus eventBus,
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] string search = "",[FromQuery] bool enabled = true)
        {
            var query = new UserPaginationQuery(pageIndex, pageSize, search,enabled);
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
            [FromBody] DeleteUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }
    }
}
