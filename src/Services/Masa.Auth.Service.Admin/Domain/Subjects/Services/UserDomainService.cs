// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await EventBus.PublishAsync(new AddUserDomainEvent(user));
    }

    public async Task UpdateAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
        await EventBus.PublishAsync(new UpdateUserDomainEvent(user));
    }

    public async Task RemoveAsync(User user)
    {
        await EventBus.PublishAsync(new RemoveUserDomainEvent(user));
    }

    public async Task AddRangeAsync(IEnumerable<User> users)
    {
        await EventBus.PublishAsync(new AddRangeUserDomainEvent(users));
    }

    public async Task UpdateRangeAsync(IEnumerable<User> users)
    {
        await EventBus.PublishAsync(new UpdateRangeUserDomainEvent(users));
    }

    public async Task UpdateAuthorizationAsync(IEnumerable<Guid> roles)
    {
        await EventBus.PublishAsync(new UpdateUserAuthorizationDomainEvent(roles));
    }


    public async Task<List<Guid>> GetPermissionIdsAsync(Guid userId, List<Guid>? teams = null)
    {
        var query = new QueryUserPermissionDomainEvent(userId, teams);
        await EventBus.PublishAsync(query);
        return query.Permissions;
    }

    public async Task<bool> AuthorizedAsync(string appId, string code, Guid userId)
    {
        var query = new UserAuthorizedDomainEvent(appId, code, userId);
        await EventBus.PublishAsync(query);
        return query.Authorized;
    }

    public async Task VerifyRepeatAsync(string? phoneNumber, string? email, string? idCard, string? account, Guid? curUserId = default)
    {
        Expression<Func<User, bool>> condition = user => false;
        condition = condition.Or(!string.IsNullOrEmpty(account), user => user.Account == account);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), user => user.PhoneNumber == phoneNumber || user.Account == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), user => user.Email == email);
        condition = condition.Or(!string.IsNullOrEmpty(idCard), user => user.IdCard == idCard);
        condition = condition.And(curUserId is not null, user => user.Id != curUserId);

        var existUser = await _userRepository.FindAsync(condition);
        if (existUser is not null)
        {
            if (account != existUser.Account && phoneNumber != existUser.PhoneNumber && phoneNumber == existUser.Account)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existUser.PhoneNumber)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && email == existUser.Email)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && idCard == existUser.IdCard)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(account) is false && account == existUser.Account)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_EXIST, account);
        }
    }

    public UserDetailDto? UserSplicingData(User? user)
    {
        UserDetailDto? userDetailDto = null;
        if (user != null)
        {
            userDetailDto = user;
            var staff = user.Staff;
            userDetailDto.StaffId = staff?.Id;
            userDetailDto.StaffDisplayName = staff?.DisplayName;
            userDetailDto.CurrentTeamId = staff?.CurrentTeamId;
        }
        return userDetailDto;
    }
}

