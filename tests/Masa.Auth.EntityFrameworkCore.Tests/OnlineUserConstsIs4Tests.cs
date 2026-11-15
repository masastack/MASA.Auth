// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Xunit;
using Is4Types = IdentityServer4.IdentityServerConstants.PersistedGrantTypes;

namespace Masa.Auth.EntityFrameworkCore.Tests;

/// <summary>
/// OnlineUserConsts 与 IS4 的对照：Contracts 层不引用 IS4，所以这里把手写的字符串对照真实的 IS4 常量，
/// 防止字符串写错或 IS4 升级后改值，导致在线列表 / 踢人过滤静默失效。
/// </summary>
public class OnlineUserConstsIs4Tests
{
    [Fact]
    public void RefreshTokenType_MatchesIs4()
        => Assert.Equal(Is4Types.RefreshToken, OnlineUserConsts.RefreshTokenType);

    [Fact]
    public void UserConsentType_MatchesIs4()
        => Assert.Equal(Is4Types.UserConsent, OnlineUserConsts.UserConsentType);

    [Fact]
    public void PortalSessionType_DoesNotCollideWithAnyIs4Type()
    {
        var is4Types = new[]
        {
            Is4Types.AuthorizationCode,
            Is4Types.ReferenceToken,
            Is4Types.RefreshToken,
            Is4Types.UserConsent,
            Is4Types.DeviceCode,
            Is4Types.UserCode,
        };

        Assert.DoesNotContain(OnlineUserConsts.PortalSessionType, is4Types);
    }
}
