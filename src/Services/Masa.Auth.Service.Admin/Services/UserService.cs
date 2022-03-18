using Masa.Auth.Service.Admin.Dto.Subjects;

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

        private async Task<PaginationList<UserDto>> PaginationAsync(IEventBus eventBus, UserPaginationOptions options)
        {
            var query = new UserPaginationQuery(options.Page, options.PageSize, options.Search, options.Enabled);
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
