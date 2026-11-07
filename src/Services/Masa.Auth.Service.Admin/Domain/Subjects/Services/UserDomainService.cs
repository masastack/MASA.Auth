// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    readonly IUserRepository _userRepository;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly ILogger<UserDomainService> _logger;
    readonly IUserContext _userContext;
    readonly IDistributedCacheClient _cacheClient;
    readonly RoleDomainService _roleDomainService;
    readonly AuthDbContext _dbContext;
    readonly IUnitOfWork _unitOfWork;
    readonly WebhookDomainService _webhookDomainService;
    readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public UserDomainService(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        ILogger<UserDomainService> logger,
        IUserContext userContext,
        IDistributedCacheClient cacheClient,
        RoleDomainService roleDomainService,
        AuthDbContext dbContext,
        IUnitOfWork unitOfWork,
        WebhookDomainService webhookDomainService,
        IMultiEnvironmentContext multiEnvironmentContext)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _logger = logger;
        _userContext = userContext;
        _cacheClient = cacheClient;
        _roleDomainService = roleDomainService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _webhookDomainService = webhookDomainService;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    public async Task AddAsync(User user)
    {
        var (_, exception) = await VerifyRepeatAsync(user.PhoneNumber, user.Email, user.IdCard, user.Account);
        if (exception != null)
        {
            throw exception;
        }
        user = await _userRepository.AddAsync(user);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            //A filtered unique index rejected this insert (e.g. a concurrent request created the same user
            //first). Detach the unsaved graph so the scoped DbContext is not polluted - otherwise a later
            //SaveChanges in the same unit of work would try to insert it again - and rethrow.
            //Do NOT swallow the exception here: returning normally would make callers believe that
            //this user was created and hand out an entity (and Id) that does not exist in the database.
            _dbContext.DetachPendingUserGraphs(new[] { user.Id });

            //Translate into the same business error the sequential path (VerifyRepeatAsync above) would
            //have produced, so regular callers (AddUserCommand / register / import) surface a localized
            //message instead of a raw DbUpdateException. Callers that can tolerate a lost race
            //(ldap / third-party user creation) catch UserFriendlyException too and fall back to the
            //winning user themselves; when no conflicting user is found, rethrow the original exception.
            var (_, conflict) = await VerifyRepeatAsync(user.PhoneNumber, user.Email, user.IdCard, user.Account, user.Id);
            if (conflict != null)
            {
                throw conflict;
            }
            throw;
        }
        await BackgroundJobManager.EnqueueAsync(new SyncUserArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            UserId = user.Id
        });

        await _webhookDomainService.TriggerAsync(WebhookEvent.AddUser, user);
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

        await BackgroundJobManager.EnqueueAsync(new SyncUserArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            UserId = user.Id
        });

        await _webhookDomainService.TriggerAsync(WebhookEvent.UpdateUser, user);
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
            await _cacheClient.SetAsync(CacheKey.UserKey(userId), userModel);
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

        //批量写：单条 SetAsync = SET+PUB 两次串行往返，LDAP 同步批量可达上千用户；SetListAsync 一次脚本完成
        var keyValues = new Dictionary<string, UserModel?>();
        var documents = new List<UserSelectDto>();
        var roleIds = new HashSet<Guid>();
        foreach (var user in users)
        {
            var userModel = user.Adapt<UserModel>();
            userModel.Roles = user.Roles.Where(e => !e.IsDeleted).Select(e => new RoleModel { Id = e.RoleId, Name = e.Role == null ? "" : e.Role.Name, Code = e.Role == null ? "" : e.Role.Code }).ToList();
            userModel.Permissions = user.Permissions.Where(e => !e.IsDeleted).Adapt<List<SubjectPermissionRelationModel>>();
            var staff = user.Staff;
            userModel.StaffDisplayName = staff?.DisplayName;
            userModel.StaffId = staff?.Id;

            keyValues.Add(CacheKey.UserKey(user.Id), userModel);
            documents.Add(user.Adapt<UserSelectDto>());
            roleIds.UnionWith(user.Roles.Select(r => r.RoleId));
        }

        await _cacheClient.SetListAsync(keyValues);

        var result = await _autoCompleteClient.SetBySpecifyDocumentAsync(documents);
        if (!result.IsValid)
        {
            _logger.LogError(JsonSerializer.Serialize(result));
        }

        //角色限额更新合并为一次事件：handler 本身按 Contains 批量查询并 UpdateRangeAsync，天然支持多角色
        await _roleDomainService.UpdateRoleLimitAsync(roleIds);
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
        await _cacheClient.SetListAsync(map);
    }

    public async Task RemoveAsync(Guid userId)
    {
        var user = await _userRepository.GetDetailAsync(userId);

        if (user == null)
        {
            return;
        }

        if (user.IsAdmin())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ADMINISTRATOR_DELETE_ERROR);
        }

        await _userRepository.RemoveAsync(user);
        await _autoCompleteClient.DeleteAsync(userId);
        await _cacheClient.RemoveAsync<UserModel>(CacheKey.UserKey(userId));

        await _webhookDomainService.TriggerAsync(WebhookEvent.DeleteUser, user);
    }

    public async Task AddRangeAsync(List<User> users)
    {
        await Filter(users);
        if (users.Count == 0)
            return;

        try
        {
            await _userRepository.AddRangeAsync(users);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            //One conflicting user would fail the whole batch insert; fall back to one-by-one adding
            //so a single conflict only skips that user instead of the whole batch.
            _logger.LogError(ex, "Batch adding {Count} users failed by unique index conflict, fallback to one-by-one adding.", users.Count);
            await AddRangeOneByOneAsync(users);
        }

        var userIds = await _dbContext.Set<User>().Where(u => users.Select(user => user.Account).Contains(u.Account))
            .Select(u => u.Id).ToListAsync();
        await SyncUserAsync(userIds);
    }

    async Task AddRangeOneByOneAsync(List<User> users)
    {
        _dbContext.DetachPendingUserGraphs(users.Select(u => u.Id));
        foreach (var user in users)
        {
            try
            {
                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _dbContext.DetachPendingUserGraphs(new[] { user.Id });
                var existing = await FindExistingUserAsync(user.Account, user.PhoneNumber, user.Email, user.IdCard);
                if (existing == null)
                {
                    _logger.LogError(ex, "Add user {Account} failed by unique index conflict and no existing user was found, skipped.", user.Account);
                }
                else
                {
                    _logger.LogWarning(ex, "Add user {Account} lost the concurrency race, the existing user {UserId} is kept.",
                        user.Account, existing.Id);
                }
            }
        }
    }

    private async Task Filter(List<User> users)
    {
        var dbUsers = await _userRepository.GetListAsync(user =>
                users.Select(u => u.Account).Contains(user.Account) ||
                users.Select(u => u.PhoneNumber).Contains(user.PhoneNumber) ||
                users.Select(u => u.Email).Contains(user.Email)
            );

        var existingUsers = dbUsers.ToHashSet();

        //应用层按不区分大小写判重——这是业务决策（登录/查重等值口径本就是 CI，见 UserUniqueKeyQueries），
        //且比 PG 的 DB 唯一索引（大小写敏感）更严格；PG 的 DB 查询本身匹配不到大小写变体，
        //内存比对必须按应用语义而非 DB 索引口径，不要照 DB 索引放宽。
        var illegalItems = users.Where(user => existingUsers.Any(u =>
            ((string.Equals(user.Account, u.Account, StringComparison.OrdinalIgnoreCase)
                || (string.Equals(user.PhoneNumber, u.PhoneNumber, StringComparison.Ordinal) && !user.PhoneNumber.IsNullOrEmpty())
                || (string.Equals(user.Email, u.Email, StringComparison.OrdinalIgnoreCase) && !user.Email.IsNullOrEmpty()))
                && user.Id != u.Id))).ToList();

        if (illegalItems.Any())
        {
            foreach (var illegalItem in illegalItems)
            {
                _logger.LogError("There is duplicate data, check Account:{Account},PhoneNumber:{PhoneNumber},Email:{Email}.", illegalItem.Account, illegalItem.PhoneNumber, illegalItem.Email);
            }
            users.RemoveAll(user => illegalItems.Any(u => string.Equals(u.Account, user.Account, StringComparison.OrdinalIgnoreCase)));
        }

        //Batch-internal duplicates (two users in this batch conflicting with each other) would also be
        //rejected by the unique indexes - keep the first occurrence and skip the rest.
        RemoveBatchInternalDuplicates(users);
    }

    void RemoveBatchInternalDuplicates(List<User> users)
    {
        RemoveInternalDuplicates(users, u => u.Account, nameof(User.Account));
        RemoveInternalDuplicates(users, u => u.PhoneNumber, nameof(User.PhoneNumber));
        RemoveInternalDuplicates(users, u => u.Email, nameof(User.Email));
    }

    void RemoveInternalDuplicates(List<User> users, Func<User, string> keySelector, string keyName)
    {
        var removed = new List<User>();
        foreach (var group in users.GroupBy(keySelector, StringComparer.OrdinalIgnoreCase))
        {
            if (group.Key.IsNullOrEmpty() || group.Count() <= 1)
                continue;

            var skipped = group.Skip(1).ToList();
            removed.AddRange(skipped);
            _logger.LogWarning(
                "Duplicate user {KeyName} '{Key}' found in the same batch, keep {Keep} and skip: {Skipped}.",
                keyName, group.Key, group.First().Account, string.Join(',', skipped.Select(u => u.Account)));
        }
        users.RemoveAll(removed);
    }

    public async Task UpdateRangeAsync(List<User> users)
    {
        var candidates = users.ToList();
        await Filter(users);
        DetachSkippedUsers(candidates, users);
        if (users.Count == 0)
            return;

        try
        {
            await _userRepository.UpdateRangeAsync(users);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            //One conflicting user would fail the whole batch update; fall back to one-by-one updating
            //so a single conflict only skips that user instead of the whole batch.
            _logger.LogError(ex, "Batch updating {Count} users failed by unique index conflict, fallback to one-by-one updating.", users.Count);
            await UpdateRangeOneByOneAsync(users);
        }
        await SyncUserAsync(users.Select(u => u.Id));
    }

    /// <summary>
    /// <see cref="Filter"/> only removes the conflicting users from the list. On the update path those users
    /// are tracked entities that already carry the conflicting changes, so they would still be flushed by the
    /// next SaveChanges (the batch save, every per-user save of the one-by-one fallback, even the commit of the
    /// surrounding unit of work) and fail it again. Discard their pending changes so "skipped" really means skipped.
    /// </summary>
    void DetachSkippedUsers(List<User> candidates, List<User> kept)
    {
        if (candidates.Count == kept.Count)
            return;

        var keptIds = kept.Select(u => u.Id).ToHashSet();
        var skippedIds = candidates.Where(u => !keptIds.Contains(u.Id)).Select(u => u.Id).ToList();
        _dbContext.DetachPendingUserGraphs(skippedIds);
    }

    async Task UpdateRangeOneByOneAsync(List<User> users)
    {
        //Same as AddRangeOneByOneAsync: detach the whole batch first. After the failed batch
        //SaveChanges every user is still tracked as Modified, so each per-user SaveChanges below
        //would otherwise submit ALL of them again and the first conflict would repeat in every round.
        _dbContext.DetachPendingUserGraphs(users.Select(u => u.Id));
        foreach (var user in users)
        {
            try
            {
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _dbContext.DetachPendingUserGraphs(new[] { user.Id });
                _logger.LogError(ex, "Update user {Account} failed by unique index conflict, skipped.", user.Account);
            }
        }
    }

    async Task<User?> FindExistingUserAsync(string account, string? phoneNumber, string? email, string? idCard)
    {
        Expression<Func<User, bool>> condition = UserUniqueKeyQueries.AccountEquals(_dbContext, account);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), u => u.PhoneNumber == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), UserUniqueKeyQueries.EmailEquals(_dbContext, email));
        condition = condition.Or(!string.IsNullOrEmpty(idCard), u => u.IdCard == idCard);
        return await _userRepository.FindAsync(condition);
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
        condition = condition.Or(!string.IsNullOrEmpty(account),
            UserUniqueKeyQueries.AccountEquals(_dbContext, account).Or(user => user.PhoneNumber == account));
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber),
            UserUniqueKeyQueries.AccountEquals(_dbContext, phoneNumber).Or(user => user.PhoneNumber == phoneNumber));
        condition = condition.Or(!string.IsNullOrEmpty(email), UserUniqueKeyQueries.EmailEquals(_dbContext, email));
        condition = condition.Or(!string.IsNullOrEmpty(idCard), user => user.IdCard == idCard);
        condition = condition.And(curUserId is not null, user => user.Id != curUserId);

        var existUser = await _userRepository.FindAsync(condition);
        UserFriendlyException? exception = default;
        if (existUser is not null)
        {
            //上面的 DB 条件已按不区分大小写命中，这里的内存确认必须同口径，
            //否则会出现“DB 判重、内存放行”的缺口。
            if (!string.Equals(account, existUser.Account, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(phoneNumber, existUser.PhoneNumber, StringComparison.Ordinal)
                && (string.Equals(phoneNumber, existUser.Account, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(account, existUser.PhoneNumber, StringComparison.Ordinal)))
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_PHONE_NUMBER_EXIST, phoneNumber ?? account!);
            if (string.IsNullOrEmpty(phoneNumber) is false && string.Equals(phoneNumber, existUser.PhoneNumber, StringComparison.Ordinal))
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && string.Equals(email, existUser.Email, StringComparison.OrdinalIgnoreCase))
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && string.Equals(idCard, existUser.IdCard, StringComparison.Ordinal))
                exception = new UserFriendlyException(UserFriendlyExceptionCodes.USER_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(account) is false && string.Equals(account, existUser.Account, StringComparison.OrdinalIgnoreCase))
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

