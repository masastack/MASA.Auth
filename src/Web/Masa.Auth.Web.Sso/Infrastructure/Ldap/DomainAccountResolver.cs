// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Constants;
using Masa.Utils.Ldap.Novell;
using Masa.Utils.Ldap.Novell.Entries;

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// 将域账号解析为 AD 的 ObjectGuid 与禁用态；须在 AD 确认存在且未禁用，断言不能替代实查。
/// 60s 缓存减轻 AD 负载（禁用态最坏延迟 60s），键按账号小写归一。
/// </summary>
public class DomainAccountResolver
{
    static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    /// <summary>解析结果缓存条目（含已禁用账号，避免反复打 AD）。</summary>
    public sealed record ResolvedAccount(string Account, string ObjectGuid, bool IsDisabled);

    readonly IThirdPartyIdpService _thirdPartyIdpService;
    readonly ILdapFactory _ldapFactory;
    readonly IConnectionMultiplexer _mux;
    readonly ILogger<DomainAccountResolver> _logger;

    public DomainAccountResolver(
        IThirdPartyIdpService thirdPartyIdpService,
        ILdapFactory ldapFactory,
        IConnectionMultiplexer mux,
        ILogger<DomainAccountResolver> logger)
    {
        _thirdPartyIdpService = thirdPartyIdpService;
        _ldapFactory = ldapFactory;
        _mux = mux;
        _logger = logger;
    }

    /// <summary>
    /// 解析账号；<paramref name="account"/> 必须是已归一化的 sAMAccountName。
    /// 返回 <c>null</c> 表示 AD 中不存在该账号。
    /// </summary>
    public virtual async Task<ResolvedAccount?> ResolveAsync(string account)
    {
        var cacheKey = $"ldap:assert:{account.ToLowerInvariant()}";
        var cached = await TryReadCacheAsync(cacheKey);
        if (cached is not null)
        {
            return cached;
        }

        var session = await LdapRootConnection.OpenAsync(_thirdPartyIdpService, _ldapFactory);

        var ldapUser = await session.Provider.GetUserByUserNameAsync(account);
        if (ldapUser is null || string.IsNullOrEmpty(ldapUser.ObjectGuid))
        {
            return null;
        }

        var resolved = new ResolvedAccount(
            ldapUser.SamAccountName ?? account,
            ldapUser.ObjectGuid,
            ldapUser.IsAccountDisabled);

        await TryWriteCacheAsync(cacheKey, resolved);
        return resolved;
    }

    async Task<ResolvedAccount?> TryReadCacheAsync(string key)
    {
        try
        {
            var value = await _mux.GetDatabase().StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            var parts = value.ToString().Split('|');
            if (parts.Length != 3 || !bool.TryParse(parts[2], out var disabled))
            {
                return null;
            }

            return new ResolvedAccount(parts[0], parts[1], disabled);
        }
        catch (Exception ex)
        {
            // 缓存故障时直查 AD，不因 Redis 拒绝解析
            _logger.LogWarning(ex, "Domain account cache read failed, falling back to LDAP");
            return null;
        }
    }

    async Task TryWriteCacheAsync(string key, ResolvedAccount account)
    {
        try
        {
            await _mux.GetDatabase().StringSetAsync(
                key,
                $"{account.Account}|{account.ObjectGuid}|{account.IsDisabled}",
                CacheTtl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Domain account cache write failed");
        }
    }
}
