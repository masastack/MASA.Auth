// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// 域账号名（sAMAccountName）的纯函数：校验与归一化。
/// 账号名会进入 LDAP 过滤器与 Redis 键，凡是接收外部输入的入口（<c>grant_type=ldap</c> 的 userName、断言的 sub）都必须先过这里。
/// </summary>
public static class DomainAccountName
{
    /// <summary>sAMAccountName 的最大长度（AD 允许的上限）。</summary>
    const int MaxLength = 256;

    /// <summary>LDAP 过滤器/DN 元字符及 Redis 键分隔符；出现任一个都拒绝。</summary>
    const string ForbiddenChars = "*()\\/\"[]:;|=,+?<>@";

    /// <summary>
    /// 账号名可否进入 LDAP 过滤器与缓存键；须剔除 LDAP 元字符等，否则 <c>sub=*</c> 等可匹配任意条目。
    /// </summary>
    public static bool IsValid([NotNullWhen(true)] string? account)
    {
        if (string.IsNullOrWhiteSpace(account) || account.Length > MaxLength)
        {
            return false;
        }

        foreach (var ch in account)
        {
            if (char.IsControl(ch) || ForbiddenChars.Contains(ch))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>归一账号标识（去 <c>DOMAIN\</c>、<c>@domain</c>），得到 sAMAccountName。</summary>
    public static string Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var value = raw.Trim();

        var slash = value.LastIndexOf('\\');
        if (slash >= 0 && slash < value.Length - 1)
        {
            value = value[(slash + 1)..];
        }

        var at = value.IndexOf('@');
        if (at > 0)
        {
            value = value[..at];
        }

        return value.Trim();
    }
}
