// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class UserService : RestServiceBase
    {
        public UserService(IServiceCollection services) : base(services, "api/user")
        {
            MapGet(FindByAccountAsync);
            MapPost(ValidateByAccountAsync);
            MapPost(Visit);
            MapGet(VisitedList);
        }

        private async Task<PaginationDto<UserDto>> GetListAsync(IEventBus eventBus, HttpContext context, IUserContext userContext, GetUsersDto user)
        {
            var d = context;
            var userId = userContext.GetUserId<Guid>();
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

        private async Task<UserDto> AddExternalAsync(IEventBus eventBus, [FromBody] AddUserDto dto)
        {
            dto.Enabled = true;
            dto.Password = DefaultUserAttributes.Password;
            if (dto.Gender == default) dto.Gender = GenderTypes.Male;
            if (string.IsNullOrEmpty(dto.Avatar))
            {
                if (dto.Gender == GenderTypes.Male) dto.Avatar = DefaultUserAttributes.MaleAvatar;
                else dto.Avatar = DefaultUserAttributes.FemaleAvatar;
            }
            if (string.IsNullOrEmpty(dto.DisplayName)) dto.DisplayName = dto.Name;
            var command = new AddUserCommand(dto);
            await eventBus.PublishAsync(command);
            return command.NewUser;
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

        private async Task<bool> ValidateByAccountAsync(IEventBus eventBus, [FromBody] UserAccountValidateDto accountValidateDto)
        {
            var validateCommand = new ValidateByAccountCommand(accountValidateDto.Account, accountValidateDto.Password);
            await eventBus.PublishAsync(validateCommand);
            return validateCommand.Result;
        }

        private async Task<UserDetailDto> FindByAccountAsync(IEventBus eventBus, [FromQuery] string account)
        {
            var query = new FindUserByAccountQuery(account);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task Visit(IEventBus eventBus, [FromBody] AddUserVisitedDto addUserVisitedDto)
        {
            var visitCommand = new UserVisitedCommand(addUserVisitedDto.UserId, addUserVisitedDto.Url);
            await eventBus.PublishAsync(visitCommand);
        }

        private async Task<List<UserVisitedDto>> VisitedList(IEventBus eventBus, [FromQuery] Guid userId)
        {
            var visitListQuery = new UserVisitedListQuery(userId);
            await eventBus.PublishAsync(visitListQuery);
            return visitListQuery.Result;
        }
    }
}
