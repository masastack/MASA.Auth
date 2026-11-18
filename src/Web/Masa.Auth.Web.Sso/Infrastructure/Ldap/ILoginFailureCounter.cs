// Copyright (c) MASA Stack Authors. Licensed under the Apache License, Version 2.0.

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// 登录失败计数存储抽象。窗口自首次失败起算、到期键消失；后续自增不得顺延窗口；自增与设窗须原子。
/// </summary>
public interface ILoginFailureCounter
{
    /// <summary>
    /// 原子自增并返回自增后的值。仅当该键此前不存在（或没有窗口）时才设置 <paramref name="window"/>。
    /// </summary>
    Task<long> IncrementAsync(string key, TimeSpan window);

    /// <summary>读取当前计数；键不存在返回 0。</summary>
    Task<long> GetAsync(string key);

    /// <summary>清除计数。</summary>
    Task ResetAsync(string key);
}
