// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using System.Text.RegularExpressions;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Service.Admin.Infrastructure.Attributes;
using Masa.Auth.Service.Admin.Infrastructure.Extensions;
using Masa.Auth.Service.Admin.Services;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 在线用户接口的鉴权约束。鉴权不在处理方法里做，而是靠方法上的特性（进入 endpoint 元数据后由框架执行），
/// 所以"特性还在"和"路径推导出的权限码仍与种子权限对得上"都是需要固定住的约束——
/// 任何一个被无意改掉，接口会静默变成"匿名可访问"或"所有人（含 admin）都 403"，没有编译期或运行期的提示。
/// </summary>
public class OnlineUserAuthorizationTests
{
    static MethodInfo Endpoint(string name) =>
        typeof(OnlineUserService).GetMethod(name, BindingFlags.Instance | BindingFlags.Public)
        ?? throw new InvalidOperationException($"endpoint method {name} not found");

    [Fact]
    public void KickUserAsync_RequiresPermissionCode_ViaMasaAuthorize_WithoutExplicitCode()
    {
        var attribute = Assert.Single(Endpoint(nameof(OnlineUserService.KickUserAsync)).GetCustomAttributes<MasaAuthorizeAttribute>());

        // 不写 Code：由 CodeAuthorizationMiddlewareResultHandler 按请求路径推导（与 TeamService.CreateAsync 一致）。
        // 显式 Code 不会被加 {appId}. 前缀，而用户的权限码带前缀，写了会永远匹配不上
        Assert.Equal(string.Empty, attribute.Code);
        // 也不能按角色放行：Roles 会绕过权限码校验
        Assert.True(string.IsNullOrEmpty(attribute.Roles));
        Assert.Empty(Endpoint(nameof(OnlineUserService.KickUserAsync)).GetCustomAttributes<AllowAnonymousAttribute>());
    }

    [Theory]
    [InlineData(nameof(OnlineUserService.GetListAsync))]
    [InlineData(nameof(OnlineUserService.GetSessionsAsync))]
    public void QueryEndpoints_RequireLoginOnly(string methodName)
    {
        // Masa 的 RouteHandlerBuilder 在 DisableAutoMapRoute 下不会应用到显式 MapGet，
        // 所以必须靠方法上的特性才有登录校验；查询接口只要求登录，不要求权限码
        var method = Endpoint(methodName);
        var attribute = Assert.Single(method.GetCustomAttributes<AuthorizeAttribute>());
        Assert.IsNotType<MasaAuthorizeAttribute>(attribute);
        Assert.Empty(method.GetCustomAttributes<AllowAnonymousAttribute>());
    }

    [Fact]
    public void KickPermissionCode_MatchesTheCodeDerivedFromTheRoute()
    {
        // 复刻 CodeAuthorizationMiddlewareResultHandler 的默认规则（路径 → 小写、分隔符转 '.'；{appId}. 前缀由下面的 ReplenishCode 提供）
        var path = $"/{OnlineUserService.BaseRoute}/{OnlineUserService.KickRoute}";
        var derived = Regex.Replace(Regex.Replace(path, @"\\", "."), "/", ".").Trim('.').ToLower();

        Assert.Equal(ElementPermissionCodes.ApiOnlineUserKick, derived);
    }

    [Fact]
    public void SeededKickPermission_IsMatchedByTheRequestCode_ThroughTheRealMatcher()
    {
        const string serviceId = "auth-service-id";
        var path = $"/{OnlineUserService.BaseRoute}/{OnlineUserService.KickRoute}";
        var requestCode = $"{serviceId}." + Regex.Replace(Regex.Replace(path, @"\\", "."), "/", ".").Trim('.').ToLower();

        // 与 AuthSeedData 里的种子参数一致
        var seeded = new Masa.Auth.Domain.Permissions.Aggregates.Permission(
            "auth", serviceId, "OnlineUserKick", ElementPermissionCodes.ApiOnlineUserKick, "", "", 1, PermissionTypes.Api);

        // 用户的 API 权限码列表 = 其拥有的 Api 权限的 ReplenishCode（见 QueryHandler.GetUserApiPermissionCodeQueryAsync）
        var userAllowCodes = new[] { seeded.ReplenishCode };

        Assert.True(userAllowCodes.WildCardContains(requestCode));
        // 没有踢人权限的用户（只有别的权限码）不能放行
        Assert.False(new[] { $"{serviceId}.api.team.create" }.WildCardContains(requestCode));
        Assert.False(Array.Empty<string>().WildCardContains(requestCode));
    }
}
