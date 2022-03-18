namespace Masa.Auth.Service.Admin.Services
{
    public class UserService : ServiceBase
    {
        public UserService(IServiceCollection services) : base(services, "api/user")
        {
            MapGet(PaginationAsync);
            MapPost(AddUserAsync);
            MapPut(EditUserAsync);
            MapDelete(DeleteUserAsync);
        }

        private async Task<PaginationDto<UserDto>> PaginationAsync(IEventBus eventBus, GetUsersDto options)
        {
            var query = new UserPaginationQuery(options.Page, options.PageSize, options.Name, options.PhoneNumber, options.Email, options.Enabled);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<UserDetailDto> GetUserDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid userId)
        {
            var query = new UserDetailQuery(userId);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddUserAsync(
            IEventBus eventBus,
            [FromBody] AddUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }

        private async Task EditUserAsync(
            IEventBus eventBus,
            [FromBody] UpdateUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }

        private async Task DeleteUserAsync(
            IEventBus eventBus,
            [FromBody] RemoveUserCommand command)
        {
            await eventBus.PublishAsync(command);
        }
    }
}
