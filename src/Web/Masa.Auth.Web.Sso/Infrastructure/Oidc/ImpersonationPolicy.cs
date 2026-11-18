// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Oidc;

/// <summary>RFC 8693 模拟登录纯判定（offline_access、amr 禁用清单、act 载荷），与 IS4 解耦便于单测。</summary>
public static class ImpersonationPolicy
{
    /// <summary>是否请求 <c>offline_access</c>（IS4 已解析 scope 或 raw scope 任一即可）。</summary>
    public static bool RequestsOfflineAccess(IEnumerable<string>? requestedScopes, string? rawScope)
    {
        if (requestedScopes?.Any(s => string.Equals(s, OidcConstants.StandardScopes.OfflineAccess, StringComparison.Ordinal)) == true)
        {
            return true;
        }

        return rawScope?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Any(s => string.Equals(s, OidcConstants.StandardScopes.OfflineAccess, StringComparison.Ordinal)) == true;
    }

    /// <summary>actor <c>amr</c> 中命中禁用清单的项；空表示允许。</summary>
    public static IReadOnlyList<string> FindDeniedAuthenticationMethods(
        IEnumerable<string> actorAuthenticationMethods,
        IEnumerable<string> deniedMethods)
    {
        var denied = deniedMethods.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return actorAuthenticationMethods
            .Where(denied.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>act claim 的载荷（RFC 8693 §4.1：<c>{"sub":"&lt;actorUserId&gt;"}</c>）。</summary>
    public static string BuildActClaim(Guid actorUserId)
        => JsonSerializer.Serialize(new Dictionary<string, string> { ["sub"] = actorUserId.ToString() });
}
