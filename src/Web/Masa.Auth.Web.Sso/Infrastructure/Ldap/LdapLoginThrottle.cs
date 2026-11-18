// Copyright (c) MASA Stack Authors. Licensed under the Apache License, Version 2.0.

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// <c>grant_type=ldap</c> 登录失败限流（5 次 / 30 分钟）；独立 Redis 键前缀，避免与 Admin 登录计数键类型冲突（WRONGTYPE）。
/// 窗口自首次失败起算、由 <see cref="ILoginFailureCounter"/> 键 TTL 表达，后续失败不得顺延 TTL；仅跟踪合法账号名；Redis 故障时不阻断登录。
/// </summary>
public class LdapLoginThrottle
{
    public const int MaxLoginErrorCount = 5;
    public const int LoginFreezeMinutes = 30;

    const string KeyPrefix = "ldap_login_fail:";

    static readonly TimeSpan FreezeWindow = TimeSpan.FromMinutes(LoginFreezeMinutes);

    /// <summary>冻结错误码，与 Admin <c>LOGIN_FREEZE</c> 同值（字面量，避免与 Contracts 同名冲突）。</summary>
    const string LoginFreezeErrorCode = "LoginFreeze";

    readonly ILoginFailureCounter _counter;
    readonly ILogger<LdapLoginThrottle> _logger;

    public LdapLoginThrottle(ILoginFailureCounter counter, ILogger<LdapLoginThrottle> logger)
    {
        _counter = counter;
        _logger = logger;
    }

    /// <summary>已冻结则抛 <see cref="UserFriendlyException"/>；计数存储故障时不阻断登录。</summary>
    public virtual async Task EnsureNotFrozenAsync(string? account)
    {
        if (!IsTrackable(account))
        {
            return;
        }

        long count;
        try
        {
            count = await _counter.GetAsync(KeyOf(account));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LDAP login throttle check skipped");
            return;
        }

        if (count >= MaxLoginErrorCount)
        {
            throw new UserFriendlyException(errorCode: LoginFreezeErrorCode);
        }
    }

    /// <summary>记一次失败（日志不写账号名）。</summary>
    public virtual async Task RecordFailureAsync(string? account)
    {
        if (!IsTrackable(account))
        {
            return;
        }

        try
        {
            await _counter.IncrementAsync(KeyOf(account), FreezeWindow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LDAP login throttle record skipped");
        }
    }

    public virtual async Task ResetAsync(string? account)
    {
        if (!IsTrackable(account))
        {
            return;
        }

        try
        {
            await _counter.ResetAsync(KeyOf(account));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LDAP login throttle reset skipped");
        }
    }

    static bool IsTrackable([NotNullWhen(true)] string? account)
        => DomainAccountName.IsValid(account);

    // 账号唯一性按不区分大小写生效，键统一小写，避免 Admin/admin 等变体各自计数绕过冻结
    static string KeyOf(string account) => $"{KeyPrefix}{account.ToLowerInvariant()}";
}
