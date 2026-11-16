// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>客户端属性 domain_assertion.account_pattern：| 分隔通配，全串锚定（勿用未锚定子串匹配）。</summary>
public static class DomainAccountMatching
{
    /// <summary>JWT Bearer 断言 grant 读取的账号白名单键；未配则不限制。</summary>
    public const string AccountPatternProperty = "domain_assertion.account_pattern";

    static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>空 pattern 表示不限制。</summary>
    public static bool IsAllowed(string? pattern, string account)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return true;
        }

        return pattern
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(p => Matches(p, account));
    }

    /// <summary>单个模式的通配匹配，全串锚定。</summary>
    public static bool Matches(string pattern, string value)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return false;
        }

        var regex = "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*", StringComparison.Ordinal)
            .Replace("\\?", ".", StringComparison.Ordinal) + "$";

        return Regex.IsMatch(value, regex, RegexOptions.IgnoreCase, RegexTimeout);
    }
}
