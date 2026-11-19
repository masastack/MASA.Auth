// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityModel;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Xunit;
using BuildingBlocksGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 单活 App 会话策略的常量约束。策略对 token-only 的判定是语义的（见 SSO
/// <c>CustomTokenRequestValidator.RevokeStaleAppSessionsAsync</c>：<c>ValidatedRequest.SessionId</c>
/// 为空 + 两个显式排除），谓词本身在 SSO 进程内、仓库内无法执行；这里锚定两个排除项常量的
/// 字面量——它们若随包升级漂移，排除会静默失效：refresh_token 轮换会被误当新登录（正在续期的
/// 会话自杀），token-exchange 模拟登录会误杀被模拟用户的真实会话。
/// </summary>
public class SingleActiveAppSessionPolicyTests
{
    [Fact]
    public void RefreshTokenGrantLiteral_IsStable()
    {
        // SSO 以此常量识别轮换请求（轮换的 ValidatedRequest.SessionId 同样为空，靠它显式排除）
        Assert.Equal("refresh_token", OidcConstants.GrantTypes.RefreshToken);
    }

    [Fact]
    public void TokenExchangeGrantLiteral_IsStable()
    {
        // SSO 以此常量识别管理员模拟登录（策略例外：不杀被模拟者的真实会话）。
        // 旧的 impersonation 扩展授权已在 #1499 改造为 RFC 8693 token-exchange 并从包中删除；
        // token-exchange 同为扩展授权而非 IS4 内置流，若有人误把它改成 IS4 内置 grant 的值
        // （password 等），会改变排除语义——一并锚定。
        // 与 TokenExchangeGrantValidator 的 offline_access 禁令互为冗余保险：禁令挡住
        // refresh_token 签发，排除项兜底禁令被放宽后的场景
        Assert.Equal("urn:ietf:params:oauth:grant-type:token-exchange", BuildingBlocksGrantType.TOKEN_EXCHANGE);
        Assert.NotEqual(OidcConstants.GrantTypes.Password, BuildingBlocksGrantType.TOKEN_EXCHANGE);
    }

    [Fact]
    public void RefreshTokenGrantType_MatchesPersistedGrantType()
    {
        // grant type "refresh_token" 与 PersistedGrant 行 Type 的 refresh_token 是两个概念共值：
        // 单活策略靠前者识别轮换、靠后者定位被清的行，两者若分叉，轮换排除将保护不了任何行
        Assert.Equal(OidcConstants.GrantTypes.RefreshToken, OnlineUserConsts.RefreshTokenType);
    }
}
