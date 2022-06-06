// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class UserService : RestServiceBase
    {
        public UserService(IServiceCollection services) : base(services, "api/user")
        {
        }

        private async Task<PaginationDto<UserDto>> GetListAsync(IEventBus eventBus, GetUsersDto user)
        {
            var query = new UsersQuery(user.Page, user.PageSize, user.UserId, user.Enabled, user.StartTime, user.EndTime);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<UserDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new UserDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<UserSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string search)
        {
            var query = new UserSelectQuery(search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddExternalAsync(IEventBus eventBus, [FromBody] AddUserDto dto)
        {
            dto.Enabled = true;
            dto.Password = "123";
            if (string.IsNullOrEmpty(dto.Avatar)) dto.Avatar = "";
            if (dto.Gender == default) dto.Gender = GenderTypes.Male;

            await eventBus.PublishAsync(new AddUserCommand(dto));
        }

        private async Task AddAsync(IEventBus eventBus, [FromBody] AddUserDto dto)
        {
            await eventBus.PublishAsync(new AddUserCommand(dto));
        }

        private async Task UpdateAsync(
            IEventBus eventBus,
            [FromBody] UpdateUserDto dto)
        {
            await eventBus.PublishAsync(new UpdateUserCommand(dto));
        }

        public async Task UpdateAuthorizationAsync(IEventBus eventBus,
            [FromBody] UpdateUserAuthorizationDto dto)
        {
            await eventBus.PublishAsync(new UpdateUserAuthorizationCommand(dto));
        }

        private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemoveUserDto dto)
        {
            await eventBus.PublishAsync(new RemoveUserCommand(dto));
        }
    }
}
