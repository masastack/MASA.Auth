// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    readonly IUserRepository _userRepository;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly ILogger<UserDomainService> _logger;
    readonly IUserContext _userContext;
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly RoleDomainService _roleDomainService;
    readonly AuthDbContext _dbContext;
    readonly IUnitOfWork _unitOfWork;

    public UserDomainService(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        ILogger<UserDomainService> logger,
        IUserContext userContext,
        IMultilevelCacheClient multilevelCacheClient,
        RoleDomainService roleDomainService,
        AuthDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _logger = logger;
        _userContext = userContext;
        _multilevelCacheClient = multilevelCacheClient;
        _roleDomainService = roleDomainService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(User user)
    {
        var (_, exception) = await VerifyRepeatAsync(user.PhoneNumber, user.Email, user.IdCard, user.Account);
        if (exception != null)
        {
            throw exception;
        }
        user = await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        await SyncUserAsync(user.Id);
    }

    public async Task UpdateAsync(User user)
    {
        var (_, exception) = await VerifyRepeatAsync(user.PhoneNumber, user.Email, user.IdCard, user.Account, user.Id);
        if (exception != null)
        {
            throw exception;
        }
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        await SyncUserAsync(user.Id);
    }

    public async Task SyncUserAsync(Guid userId)
    {
        var user = await _userRepository.GetDetailAsync(userId);
        if (user is not null)
        {
            var userModel = user.Adapt<UserModel>();
            userModel.Roles = user.Roles.Where(e => !e.IsDeleted).Select(e => new RoleModel { Id = e.RoleId, Name = e.Role == null ? "" : e.Role.Name, Code = e.Role == null ? "" : e.Role.Code }).ToList();
            userModel.Permissions = user.Permissions.Where(e => !e.IsDeleted).Adapt<List<SubjectPermissionRelationModel>>();
            var staff = user.Staff;
            userModel.StaffDisplayName = staff?.DisplayName;
            userModel.StaffId = staff?.Id;

            await _multilevelCacheClient.SetAsync(CacheKey.UserKey(userId), userModel);

            var result = await _autoCompleteClient.SetBySpecifyDocumentAsync(user.Adapt<UserSelectDto>());
            if (!result.IsValid)
            {
                _logger.LogError(JsonSerializer.Serialize(result));
            }

            await _roleDomainService.UpdateRoleLimitAsync(user.Roles.Select(r => r.RoleId));
        }
    }

    public async Task SyncUserAsync(IEnumerable<Guid> userIds)
    {
        var users = await _dbContext.Set<User>().Where(u => userIds.Contains(u.Id))
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();

        foreach (var user in users)
        {
            var userModel = user.Adapt<UserModel>();
            userModel.Roles = user.Roles.Where(e => !e.IsDeleted).Select(e => new RoleModel { Id = e.RoleId, Name = e.Role == null ? "" : e.Role.Name, Code = e.Role == null ? "" : e.Role.Code }).ToList();
            userModel.Permissions = user.Permissions.Where(e => !e.IsDeleted).Adapt<List<SubjectPermissionRelationModel>>();
            var staff = user.Staff;
            userModel.StaffDisplayName = staff?.DisplayName;
            userModel.StaffId = staff?.Id;

            await _multilevelCacheClient.SetAsync(CacheKey.UserKey(user.Id), userModel);

            var result = await _autoCompleteClient.SetBySpecifyDocumentAsync(user.Adapt<UserSelectDto>());
            if (!result.IsValid)
            {
                _logger.LogError(JsonSerializer.Serialize(result));
            }

            await _roleDomainService.UpdateRoleLimitAsync(user.Roles.Select(r => r.RoleId));
        }
    }

    public async Task SyncUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userIds = users.Select(u => u.Id).ToList();
        var map = new Dictionary<string, UserModel?>();
        foreach (var user in users)
        {
            var userModel = user.Adapt<UserModel>();
            userModel.StaffDisplayName = user.Staff?.DisplayName ?? "";
            userModel.StaffId = user.Staff?.Id;
            map.Add(CacheKey.UserKey(user.Id), userModel);
            await _autoCompleteClient.SetBySpecifyDocumentAsync(user.Adapt<UserSelectDto>());
        }
        await _multilevelCacheClient.SetListAsync(map);
    }

    public async Task RemoveAsync(Guid userId)
    {
        var user = await _userRepository.GetDetailAsync(userId);
        if (user.IsAdmin())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ADMINISTRATOR_DELETE_ERROR);
        }
        if (user.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CURRENT_USER_DELETE_ERROR);
        }

        await _userRepository.RemoveAsync(user);
        await _autoCompleteClient.DeleteAsync(userId);
        await _multilevelCacheClient.RemoveAsync<UserModel>(CacheKey.UserKey(userId));
    }

    public async Task AddRangeAsync(List<User> users)
    {
        await Filter(users);
        await _userRepository.AddRangeAsync(users);
        await _unitOfWork.SaveChangesAsync();
        var userIds = await _dbContext.Set<User>().Where(u => users.Select(user => user.Account).Contains(u.Account))
            .Select(u => u.Id).ToListAsync();
        await SyncUserAsync(userIds);
    }

    private async Task Filter(List<User> users)
    {
        var dbUsers = await _userRepository.GetListAsync(user =>
                users.Select(u => u.Account).Contains(user.Account) ||
                users.Select(u => u.PhoneNumber).Contains(user.PhoneNumber) ||
                users.Select(u => u.Email).Contains(user.Email)
            );

        var existingUsers = dbUsers.ToHashSet();

        var illegalItems = users.Where(user => existingUsers.Any(u => (user.Account == u.Account || (user.PhoneNumber == u.PhoneNumber && !user.PhoneNumber.IsNullOrEmpty())
                || (user.Email == u.Email && !user.Email.IsNullOrEmpty())) && user.Id != u.Id)).ToList();

        if (illegalItems.Any())
        {
            foreach (var illegalItem in illegalItems)
            {
                _logger.LogError("There is duplicate data, check Account:{Account},PhoneNumber:{PhoneNumber},Email:{Email}.", illegalItem.Account, illegalItem.PhoneNumber, illegalItem.Email);
            }
            users.RemoveAll(user => illegalItems.Any(u => u.Account == user.Account));
        }
    }

    public async Task UpdateRangeAsync(List<User> users)
    {
        await Filter(users);
        await _userRepository.UpdateRangeAsync(users);
        await _unitOfWork.SaveChangesAsync();
        await SyncUserAsync(users.Select(u => u.Id));
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

    public async Task<(User?, UserFriendlyException?)> VerifyRepeatAsync(string? phoneNumber, string? email, string? idCard, string? account, Guid? curUserId = default)
    {
        Expression<Func<User, bool>> condition = user => false;
        condition = condition.Or(!string.IsNullOrEmpty(account), user => user.Account == account);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), user => user.PhoneNumber == phoneNumber || user.Account == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), user => user.Email == email);
        condition = condition.Or(!string.IsNullOrEmpty(idCard), user => user.IdCard == idCard);
        condition = condition.And(curUserId is not null, user => user.Id != curUserId);

        var existUser = await _userRepository.FindAsync(condition);
        UserFriendlyException? exception = default;
        if (existUser is not null)
        {
            if (account != existUser.Account && phoneNumber != existUser.PhoneNumber && phoneNumber == existUser.Account)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existUser.PhoneNumber)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && email == existUser.Email)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && idCard == existUser.IdCard)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(account) is false && account == existUser.Account)
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_EXIST, account);
        }
        return (existUser, exception);
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

