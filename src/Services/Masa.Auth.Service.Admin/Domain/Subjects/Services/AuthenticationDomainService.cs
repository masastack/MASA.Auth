// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

/// <summary>
/// 账号密码/LDAP 登录校验的领域服务。从 CommandHandler 迁入:
/// 登录是认证域的核心行为,不属于某个具体命令处理器。
/// </summary>
public class AuthenticationDomainService : DomainService
{
    const int MaxLoginErrorCount = 5;
    const int LoginFreezeMinutes = 30;

    readonly IMasaConfiguration _masaConfiguration;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILdapFactory _ldapFactory;
    readonly LdapDomainService _ldapDomainService;
    readonly IUserRepository _userRepository;
    readonly UserDomainService _userDomainService;
    readonly AuthDbContext _dbContext;
    readonly ILogger<AuthenticationDomainService> _logger;

    public AuthenticationDomainService(
        IMasaConfiguration masaConfiguration,
        IDistributedCacheClient distributedCacheClient,
        ILdapIdpRepository ldapIdpRepository,
        ILdapFactory ldapFactory,
        LdapDomainService ldapDomainService,
        IUserRepository userRepository,
        UserDomainService userDomainService,
        AuthDbContext dbContext,
        ILogger<AuthenticationDomainService> logger)
    {
        _masaConfiguration = masaConfiguration;
        _distributedCacheClient = distributedCacheClient;
        _ldapIdpRepository = ldapIdpRepository;
        _ldapFactory = ldapFactory;
        _ldapDomainService = ldapDomainService;
        _userRepository = userRepository;
        _userDomainService = userDomainService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserDetailDto?> ValidateByAccountAsync(string account, string password, bool ldapLogin)
    {
        var loginFreeze = _masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue("AppSettings:LoginFreeze", false);
        if (loginFreeze)
        {
            var loginCache = await _distributedCacheClient.GetAsync<CacheLogin>(CacheKey.AccountLoginKey(account));
            if (loginCache is not null && loginCache.LoginErrorCount >= MaxLoginErrorCount)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LOGIN_FREEZE);
            }
        }

        var effectiveAccount = account;
        if (ldapLogin)
        {
            effectiveAccount = await ValidateLdapAccountAsync(account, password, loginFreeze);
        }

        Expression<Func<User, bool>> condition = UserUniqueKeyQueries.AccountEquals(_dbContext, effectiveAccount);
        condition = condition.Or(!ldapLogin, u => u.PhoneNumber == effectiveAccount);

        var user = await _userRepository.FindWithIncludAsync(condition, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
        });

        if (user == null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.ACCOUNT_NOT_EXIST, account);
        }

        if (!user.Enabled)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ACCOUNT_DISABLED);
        }

        if (!ldapLogin)
        {
            if (!user.VerifyPassword(password))
            {
                await RecordLoginFailureAsync(account, loginFreeze);
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PASSWORD_FAILED);
            }

            if (loginFreeze)
            {
                await _distributedCacheClient.RemoveAsync<CacheLogin>(CacheKey.AccountLoginKey(account));
            }
        }

        return _userDomainService.UserSplicingData(user);
    }

    async Task<string> ValidateLdapAccountAsync(string account, string password, bool loginFreeze)
    {
        var ldaps = await _ldapIdpRepository.GetListAsync();
        if (!ldaps.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LDAP_NOT_CONFIGURED);
        }
        if (ldaps.Count() > 1)
        {
            _logger.LogWarning("There are multiple Ldap configurations, and the first configuration is used when logging in with a domain account");
        }
        var ldap = ldaps.First();
        var ldapOptions = ldap.Adapt<LdapOptions>();
        var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);

        var ldapUser = await ldapProvider.GetUserByUserNameAsync(account);
        if (ldapUser == null)
        {
            await RecordLoginFailureAsync(account, loginFreeze);
            throw new UserFriendlyException(UserFriendlyExceptionCodes.LDAP_ACCOUNT_NOTEXIST, account);
        }

        //A disabled AD account would fail the bind below with result code 49 ("invalid
        //credentials"), which is misleading — reject it up-front with an accurate message.
        if (ldapUser.IsAccountDisabled)
        {
            await RecordLoginFailureAsync(account, loginFreeze);
            throw new UserFriendlyException(UserFriendlyExceptionCodes.LDAP_ACCOUNT_DISABLED, account);
        }

        var dc = new Regex("(?<=DC=).+(?=,)", options: RegexOptions.None, matchTimeout: TimeSpan.FromSeconds(1)).Match(ldapOptions.BaseDn).Value;
        if (!await ldapProvider.AuthenticateAsync($"{dc}\\{account}", password))
        {
            await RecordLoginFailureAsync(account, loginFreeze);
            throw new UserFriendlyException(UserFriendlyExceptionCodes.LDAP_ACCOUNT_VALIDATION_FAILED, account);
        }

        return await _ldapDomainService.UpsertLdapUserAsync(ldapUser);
    }

    /// <summary>
    /// 失败计数是读-改-写,非原子;同一账号并发失败会漏计,用分段锁串行化。
    /// 锁是进程内的,多实例部署下计数是尽力而为的纵深防御。
    /// </summary>
    async Task RecordLoginFailureAsync(string account, bool loginFreeze)
    {
        if (!loginFreeze)
            return;

        var segmentLock = SegmentedAsyncGate.GetSegmentLock(account);
        await segmentLock.WaitAsync();
        try
        {
            var key = CacheKey.AccountLoginKey(account);
            var loginCache = await _distributedCacheClient.GetAsync<CacheLogin>(key)
                ?? new CacheLogin { FreezeTime = DateTimeOffset.Now.AddMinutes(LoginFreezeMinutes), Account = account };
            loginCache.LoginErrorCount++;
            await _distributedCacheClient.SetAsync(key, loginCache, loginCache.FreezeTime);
        }
        finally
        {
            segmentLock.Release();
        }
    }
}
