// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class UserQueryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly AuthDbContext _authDbContext;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private readonly IPmClient _pmClient;
    private readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;
    private readonly UserDomainService _userDomainService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEventBus _eventBus;

    public UserQueryHandler(
        IUserRepository userRepository,
        AuthDbContext authDbContext,
        IDistributedCacheClient distributedCacheClient,
        IPmClient pmClient,
        IMultiEnvironmentUserContext multiEnvironmentUserContext,
        UserDomainService userDomainService,
        IWebHostEnvironment webHostEnvironment,
        IEventBus eventBus)
    {
        _userRepository = userRepository;
        _authDbContext = authDbContext;
        _distributedCacheClient = distributedCacheClient;
        _pmClient = pmClient;
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
        _userDomainService = userDomainService;
        _webHostEnvironment = webHostEnvironment;
        _eventBus = eventBus;
    }

    #region User

    [EventHandler]
    public async Task UserHasAnyRoleQuery(UserHasAnyRoleQuery query)
    {
        var user = await _userRepository.AsQueryable().AsNoTracking().Include(x => x.UserClaims).Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Id == query.UserId);

        if (user is null)
        {
            return;
        }

        var hasRole = user.Roles.Any(r => query.RoleIds.Contains(r.RoleId));
        if (!hasRole)
        {
            var validateCommand = new ValidateDynamicRoleCommand(query.UserId, query.RoleIds);
            await _eventBus.PublishAsync(validateCommand);
            hasRole = validateCommand.Result.Any(r => r.IsValid);
        }
        query.Result = hasRole;
    }

    [EventHandler]
    public async Task UserHasRolesQuery(UserHasRolesQuery query)
    {
        var user = await _userRepository.AsQueryable().AsNoTracking().Include(x => x.UserClaims).Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.Id == query.UserId);

        if (user is null)
        {
            return;
        }

        // 获取用户直接拥有的角色ID
        var userRoleIds = user.Roles
            .Where(r => query.RoleIds.Contains(r.RoleId))
            .Select(r => r.RoleId)
            .ToList();

        // 验证动态角色
        var validateCommand = new ValidateDynamicRoleCommand(query.UserId, query.RoleIds);
        await _eventBus.PublishAsync(validateCommand);

        // 获取动态角色ID
        var dynamicRoleIds = validateCommand.Result
            .Where(r => r.IsValid)
            .Select(r => r.RoleId)
            .ToList();

        // 合并去重后返回
        query.Result = userRoleIds.Union(dynamicRoleIds).ToList();
    }

    [EventHandler]
    public async Task GetUsersAsync(UsersQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        condition = condition.And(query.Enabled is not null, user => user.Enabled == query.Enabled);
        condition = condition.And(query.StartTime is not null, user => user.CreationTime >= query.StartTime);
        condition = condition.And(query.EndTime is not null, user => user.CreationTime <= query.EndTime);
        condition = condition.And(query.UserId != Guid.Empty, user => user.Id == query.UserId);

        var users = await _userRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        var userDtos = users.Result.Select(u => (UserDto)u).ToList();
        var maskedUserDtos = userDtos.ApplyDataMasking();
        query.Result = new(users.Total, maskedUserDtos);
    }

    [EventHandler]
    public async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.GetDetailAsync(query.UserId);
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        var userDetail = _userDomainService.UserSplicingData(user);
        query.Result = userDetail!;
    }


    [EventHandler]
    public async Task FindUserByAccountAsync(FindUserByAccountQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(UserUniqueKeyQueries.AccountEquals(_authDbContext, query.Account), new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
        });
        query.Result = _userDomainService.UserSplicingData(user);
    }

    [EventHandler]
    public async Task GetUsersByAccountAsync(UsersByAccountQuery query)
    {
        var users = await _userRepository.GetListAsync(u => query.Accounts.Contains(u.Account)
                    || query.Accounts.Contains(u.PhoneNumber));
        query.Result = users.Adapt<List<UserSimpleModel>>();
    }

    [EventHandler]
    public async Task GetUserByPhoneAsync(UserByPhoneQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.PhoneNumber == query.PhoneNumber);
        query.Result = user?.Adapt<UserSimpleModel>();
    }

    [EventHandler]
    public async Task FindUserByEmailAsync(FindUserByEmailQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(UserUniqueKeyQueries.EmailEquals(_authDbContext, query.Email), new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
        });
        query.Result = _userDomainService.UserSplicingData(user);
    }

    [EventHandler]
    public async Task FindUserByPhoneNumberAsync(FindUserByPhoneNumberQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(user => user.PhoneNumber == query.PhoneNumber, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
        });
        query.Result = _userDomainService.UserSplicingData(user);
    }

    [EventHandler]
    public async Task GetUserSelectAsync(UserSelectQuery query, IAutoCompleteClient autoCompleteClient)
    {
        var response = await autoCompleteClient.GetBySpecifyDocumentAsync<UserSelectDto>(
            query.Search.TrimStart(' ').TrimEnd(' '), new AutoCompleteOptions
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Field = string.Empty
            }
        );
        query.Result = response.Data;
    }

    [EventHandler]
    public async Task GetUserRolesAsync(UserRolesQuery query)
    {
        var userRoles = await _authDbContext.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == query.UserId && ur.Role.Enabled)
            .Select(ur => new UserRoleModel
            {
                Code = ur.Role.Code,
                Name = ur.Role.Name,
                BindTime = ur.CreationTime
            })
            .ToListAsync();

        query.Result = userRoles;
    }

    [EventHandler]
    public async Task GetAllUsers(AllUsersQuery query)
    {
        var users = await _userRepository.GetAllAsync();
        query.Result = users;
    }

    [EventHandler]
    public async Task UserPortraitsAsync(UserPortraitsQuery userPortraitsQuery)
    {
        //一次批量读全部用户缓存，替代逐 id 的 N 次串行往返（调用方常见为整页在线用户）
        var keys = userPortraitsQuery.UserIds.Select(CacheKey.UserKey).ToList();
        if (keys.Count == 0)
        {
            return;
        }
        var userModels = await _distributedCacheClient.GetListAsync<UserModel>(keys);
        foreach (var userCache in userModels)
        {
            if (userCache != null)
            {
                userPortraitsQuery.Result.Add(userCache);
            }
        }
    }

    [EventHandler]
    public async Task HasPasswordAsync(HasPasswordQuery query)
    {
        query.Result = await _authDbContext.Set<User>().AnyAsync(user => user.Id == query.UserId && user.Password != null && user.Password != "");
    }

    [EventHandler(1)]
    public async Task VerifyUserRepeatAsync(VerifyUserRepeatQuery command)
    {
        var user = command.User;
        await _userDomainService.VerifyRepeatAsync(user.PhoneNumber, user.Email, user.IdCard, user.Account, user.Id);
        command.Result = true;
    }

    #endregion

    [EventHandler]
    public async Task UserVisitedListQueryAsync(UserVisitedListQuery userVisitedListQuery)
    {
        var key = CacheKey.UserVisitKey(userVisitedListQuery.UserId);
        var visited = await _distributedCacheClient.GetAsync<List<CacheUserVisited>>(key);
        if (visited != null)
        {
            var projects = await _pmClient.ProjectService.GetProjectAppsAsync(_multiEnvironmentUserContext.Environment ?? _webHostEnvironment.EnvironmentName);
            var apps = projects.SelectMany(p => p.Apps);
            //todo cache
            var menus = visited.GroupJoin(_authDbContext.Set<Permission>().Where(p => p.Type == PermissionTypes.Menu).AsEnumerable(),
                v => new { v.AppId, Url = v.Url.ToLower().Trim('/') }, p => new { p.AppId, Url = p.Url?.ToLower().Trim('/') ?? "" }, (v, p) => new
                {
                    UserVisited = v,
                    Permissions = p
                }).SelectMany(x => x.Permissions.DefaultIfEmpty(), (v, p) => new
                {
                    v.UserVisited,
                    Permission = p
                })
                .GroupJoin(apps, p => p.Permission?.AppId, a => a.Identity, (p, a) => new
                {
                    Permission = p.Permission,
                    AppModels = a
                }).SelectMany(x => x.AppModels.DefaultIfEmpty(), (p, a) =>
                   new KeyValuePair<string, string>($"{a?.Url.TrimEnd('/')}/{p.Permission?.Url.TrimStart('/')}", p.Permission?.Name ?? "")
                ).ToList();
            userVisitedListQuery.Result = menus.Select(v => new UserVisitedModel
            {
                Url = v.Key,
                Name = v.Value
            }).Where(v => !string.IsNullOrEmpty(v.Name)).ToList();
        }
    }

    [EventHandler]
    public async Task UserSystemBizDataQueryAsync(UserSystemBusinessDataQuery userSystemBusinessData)
    {
        userSystemBusinessData.Result = await _authDbContext.Set<UserSystemBusinessData>().Where(data => data.SystemId == userSystemBusinessData.SystemId
            && userSystemBusinessData.UserIds.Contains(data.UserId)).ToDictionaryAsync(item => item.UserId.ToString(), item => item.Data);
    }

    [EventHandler]
    public async Task UserClaimValuesQueryAsync(UserClaimValuesQuery userClaimValuesQuery)
    {
        userClaimValuesQuery.Result = await _authDbContext.Set<UserClaimValue>()
            .Where(c => c.UserId == userClaimValuesQuery.UserId)
            .OrderByDescending(c => c.ModificationTime)
            .ThenByDescending(c => c.CreationTime)
            .Select(c => new ClaimValue(c.Name, c.Value)).ToListAsync();

        var user = await _userRepository.FindWithIncludAsync(u => u.Id == userClaimValuesQuery.UserId, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}", nameof(User.Staff)
        });
        if (user != null)
        {
            var compatibleClaims = new List<ClaimValue>();

            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.PHONE_NUMBER))
            {
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.PHONE_NUMBER, user.PhoneNumber));
            }
            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.ACCOUNT))
            {
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.ACCOUNT, user.Account));
            }
            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.USER_NAME))
            {
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.USER_NAME, user.DisplayName));
            }
            // Sync website login token claims (AccountController.Login): all grant types get consistent roles/team/staff claims
            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.ROLES))
            {
                var roleCodes = user.Roles.Select(ur => ur.Role?.Code ?? "");
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.ROLES, JsonSerializer.Serialize(roleCodes)));
            }
            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.CURRENT_TEAM))
            {
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.CURRENT_TEAM, (user.Staff?.CurrentTeamId ?? Guid.Empty).ToString()));
            }
            if (userClaimValuesQuery.Result.All(c => c.Key != IdentityClaimConsts.STAFF))
            {
                compatibleClaims.Add(new ClaimValue(IdentityClaimConsts.STAFF, (user.Staff is { Enabled: true } ? user.Staff.Id : Guid.Empty).ToString()));
            }
            userClaimValuesQuery.Result.InsertRange(0, compatibleClaims);
        }
        // Sort the claim values by their keys in ascending order.
        userClaimValuesQuery.Result = userClaimValuesQuery.Result.OrderBy(c => c.Key).ToList();
    }

    [EventHandler]
    public async Task GetImpersonatedUserAsync(ImpersonatedUserQuery query)
    {
        var key = CacheKey.ImpersonationUserKey(query.ImpersonationToken);
        var cacheItem = await _distributedCacheClient.GetAsync<ImpersonationCacheItem>(key);
        if (cacheItem == null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_ERROR_MESSAGE);
        }

        query.Result = cacheItem;
        //await _distributedCacheClient.RemoveAsync(key);
    }
}
