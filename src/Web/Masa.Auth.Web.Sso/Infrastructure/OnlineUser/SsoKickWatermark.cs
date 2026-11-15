// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;

// 命名空间不能叫 Infrastructure.Security：同目录下的 ThirdPartyIdpGrantValidator 用相对写法
// Security.OAuth.Providers 引用 Masa.Auth.Security.OAuth.Providers，会被同名子命名空间抢占而编译失败。
namespace Masa.Auth.Web.Sso.Infrastructure.OnlineUser;

/// <summary>
/// 踢人水位线检查（SSO 门户 Cookie 拦截，挂在 OnValidatePrincipal 管道）。
///
/// 语义：Admin 踢人时写 online:kicked-at:sub:{sub} / online:kicked-at:sid:{sid}
/// （unix 毫秒，TTL 30 天，不删键）。比较的是 Cookie 的 IssuedUtc：
/// - ASP.NET Core 的滑动续期会把 IssuedUtc 更新为"最近一次续期时刻"（并非登录时刻），
///   但 OnValidatePrincipal 先于续期执行：被踢后的第一次请求就会被拒绝并 SignOut，
///   旧 Cookie 没有机会再续期，所以"IssuedUtc 早于水位线"仍然精确等价于"被踢前签发/续期的 Cookie"；
/// - 被踢前的所有旧 Cookie（含同用户其他浏览器，经 sub 水位线）持续被拒；
/// - 被踢后重新登录签发的新 Cookie 晚于水位线，自然通过（offline-only，可立即重登）。
/// </summary>
public static class SsoKickWatermark
{
    /// <summary>
    /// IS4 把浏览器会话 id 存在 Cookie 票据的 AuthenticationProperties.Items["session_id"]
    /// （DefaultUserSession.CreateSessionIdAsync），并不会往 principal 上加 "sid" claim；
    /// 这里读 claim 永远是 null，sid 级水位线就等于没生效。
    /// </summary>
    const string SessionIdKey = "session_id";

    /// <summary>
    /// 命中水位线则拒绝 principal 并强制登出。
    /// sid 级优先（只拒该浏览器），sub 级兜底（踢全部时拒该用户所有旧 Cookie）。
    /// </summary>
    public static async Task RejectKickedSessionAsync(CookieValidatePrincipalContext context)
    {
        var sub = context.Principal?.FindFirst("sub")?.Value;
        string? sid = null;
        context.Properties?.Items.TryGetValue(SessionIdKey, out sid);
        if (sub == null && sid == null)
            return;

        // 无法判定签发时刻时按"最早"处理：只要存在水位线就拒绝（fail-closed）。
        // ASP.NET Core 的 Cookie 处理器登录时一定会写入 IssuedUtc，缺失只可能是异常票据；
        // 误伤的代价仅是该用户重新登录一次（新签发的 Cookie 带 IssuedUtc，晚于水位线，自然通过）；
        // 没有水位线时不受影响
        var issuedUtc = context.Properties?.IssuedUtc ?? DateTimeOffset.MinValue;

        var db = context.HttpContext.RequestServices
            .GetRequiredService<IConnectionMultiplexer>()
            .GetDatabase();

        if (!await IsKickedAsync(db, sid, sub, issuedUtc))
            return;

        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
    }

    /// <summary>
    /// 一次 MGET 取 sid / sub 两个水位线：这是 SSO 每个已认证请求都会走的热路径，
    /// 两次串行往返会直接叠在请求延迟上（Redis 跨机房时尤其明显）。
    /// 缺的键返回 Null，与"未命中水位线"等价。
    /// </summary>
    static async Task<bool> IsKickedAsync(IDatabase db, string? sid, string? sub, DateTimeOffset issuedUtc)
    {
        // 固定顺序拼接，保证 sid/sub 与返回值下标一一对应
        var keys = new List<RedisKey>(2);
        if (sid != null) keys.Add(OnlineUserConsts.KickedAtSessionKey(sid));
        if (sub != null) keys.Add(OnlineUserConsts.KickedAtSubjectKey(sub));

        var values = await db.StringGetAsync(keys.ToArray());
        foreach (var value in values)
        {
            var valueText = (string?)value;
            if (!value.HasValue || valueText == null)
                continue;

            if (long.TryParse(valueText, out var kickedAtMs) && issuedUtc.ToUnixTimeMilliseconds() < kickedAtMs)
                return true;
        }

        return false;
    }
}
