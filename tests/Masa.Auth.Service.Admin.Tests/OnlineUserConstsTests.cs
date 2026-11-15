// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// OnlineUserConsts 里互相依赖的取值不变量。这些值分散在 Admin / SSO 两个进程里使用，
/// 靠注释"对齐"很容易漂移，这里把依赖关系固化成测试。
/// 与 IS4 常量的对照在 Masa.Auth.EntityFrameworkCore.Tests（该项目才引用 IS4）。
/// </summary>
public class OnlineUserConstsTests
{
    [Fact]
    public void CookieDependentLifetimes_ShareOneSource()
    {
        // 水位线 TTL 若短于 Cookie 寿命，被踢用户的旧 Cookie 会在水位线过期后重新生效；
        // 门户行 TTL 短于 Cookie 寿命则会让仍有效的门户会话提前被清理
        Assert.Equal(OnlineUserConsts.SsoCookieMaxLifetimeDays, OnlineUserConsts.KickedAtKeyTtlDays);
        Assert.Equal(OnlineUserConsts.SsoCookieMaxLifetimeDays, OnlineUserConsts.PortalSessionLifetimeDays);
    }

    [Fact]
    public void PortalRowColumns_FitPersistedGrantColumnLimits()
    {
        // auth.PersistedGrant: Key(200) / Type(50) / SessionId(100) / SubjectId(200)
        Assert.True(OnlineUserConsts.PortalSessionType.Length <= 50);

        // 最坏情况：Guid 格式的 SubjectId(36) + 100 字符的 SessionId 上限
        var worstCaseKey = $"{OnlineUserConsts.PortalSessionKeyPrefix}{new string('s', 36)}:{new string('x', 100)}";
        Assert.True(worstCaseKey.Length <= 200, $"portal row key may overflow Key(200): {worstCaseKey.Length}");
    }

    [Fact]
    public void WatermarkKeys_DoNotCollideBetweenSubjectAndSession()
    {
        // 同一个字符串既当 subjectId 又当 sessionId 时，两类水位线键也必须不同
        Assert.NotEqual(OnlineUserConsts.KickedAtSubjectKey("x"), OnlineUserConsts.KickedAtSessionKey("x"));
        Assert.StartsWith(OnlineUserConsts.KickedAtKeyPrefix, OnlineUserConsts.KickedAtSubjectKey("x"));
        Assert.StartsWith(OnlineUserConsts.KickedAtKeyPrefix, OnlineUserConsts.KickedAtSessionKey("x"));
    }
}
