namespace Masa.Auth.Service.Admin.Services
{
    public class UserService : ServiceBase
    {
        public UserService(IServiceCollection services) : base(services, "api/user")
        {
            MapGet(GetUsersAsync);
            MapGet(GetUserDetailAsync);
            MapGet(GetUserSelectAsync);
            MapPut(AddUserAsync);
            MapPost(UpdateUserAsync);
            MapDelete(DeleteUserAsync);
        }

        private async Task<PaginationDto<UserDto>> GetUsersAsync(IEventBus eventBus, GetUsersDto user)
        {
            var query = new UsersQuery(user.Page, user.PageSize, user.UserId, user.Enabled);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<UserDetailDto> GetUserDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new UserDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<UserSelectDto>> GetUserSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string search)
        {
            var query = new UserSelectQuery(search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddUserAsync(IEventBus eventBus, [FromBody] AddUserDto dto)
        {
            await eventBus.PublishAsync(new AddUserCommand(dto));
        }

        private async Task UpdateUserAsync(
            IEventBus eventBus,
            [FromBody] UpdateUserDto dto)
        {
            await eventBus.PublishAsync(new UpdateUserCommand(dto));
        }

        private async Task DeleteUserAsync(
            IEventBus eventBus,
            [FromBody] RemoveUserDto dto)
        {
            await eventBus.PublishAsync(new RemoveUserCommand(dto));
        }
    }
}
