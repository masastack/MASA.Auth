// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
// Masa 的 OpenIdConnect.Domain 包在全局命名空间里有同名的 PersistedGrant 实体，IS4.Models 里也有，
// 裸写 PersistedGrant 会绑错——测试里一律用别名固定为 IS4 EF 的实体（PersistedGrantDbContext 模型里的那个）
using GrantEntity = IdentityServer4.EntityFramework.Entities.PersistedGrant;

namespace Masa.Auth.EntityFrameworkCore.Tests;

/// <summary>
/// SSO <see cref="CustomTokenRequestValidator"/> 里两处 grant 清理的运行时行为，直接对真实的
/// IS4 <see cref="PersistedGrantDbContext"/>（SQLite）执行：
/// - 新登录（password 等）：单活 App 会话——清掉同 (SubjectId, ClientId) 上次登录的整条 App 续期链；
/// - refresh：修剪同一续期链里更早的已消费行，保留存活行。
///
/// 为什么必须跑真实上下文：此前单活清理写的是 <c>Set&lt;PersistedGrant&gt;()</c>，在 SSO 里裸写
/// <c>PersistedGrant</c> 会绑到 Masa 包全局命名空间里的同名实体（它只映射在 Admin 的 AuthDbContext），
/// 不在 SSO 这个 DbContext 的模型里——编译通过、运行时抛异常被 catch 吞掉，策略从未生效。
/// 这里的测试一旦再绑错类型，删除不会发生，断言立刻失败。
/// </summary>
public class CustomTokenRequestValidatorGrantCleanupTests : IDisposable
{
    const string RefreshTokenType = "refresh_token";

    readonly SqliteConnection _connection;
    readonly PersistedGrantDbContext _context;
    readonly DateTime _now = DateTime.UtcNow;

    public CustomTokenRequestValidatorGrantCleanupTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var dbOptions = new DbContextOptionsBuilder<PersistedGrantDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new PersistedGrantDbContext(dbOptions, new OperationalStoreOptions());
        _context.Database.EnsureCreated();
    }

    #region helpers

    GrantEntity Row(string key, string subjectId, string clientId, string? sessionId, bool consumed,
        string type = RefreshTokenType) => new()
    {
        Key = key,
        Type = type,
        SubjectId = subjectId,
        ClientId = clientId,
        SessionId = sessionId,
        CreationTime = _now.AddHours(-3),
        Expiration = _now.AddDays(30),
        ConsumedTime = consumed ? _now.AddHours(-1) : null,
        Data = "{}"
    };

    async Task SeedAsync(params GrantEntity[] rows)
    {
        _context.PersistedGrants.AddRange(rows);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    async Task<List<string>> RemainingKeysAsync()
        => (await _context.PersistedGrants.AsNoTracking().Select(g => g.Key).ToListAsync())
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToList();

    static List<string> Keys(params string[] keys) => keys.OrderBy(k => k, StringComparer.Ordinal).ToList();

    static Client NewClient(string clientId, TokenUsage usage = TokenUsage.OneTimeOnly, bool offline = true) => new()
    {
        ClientId = clientId,
        RefreshTokenUsage = usage,
        AllowOfflineAccess = offline
    };

    /// <summary>refresh 请求：IS4 在调用自定义验证器前已赋值 RefreshToken / Subject（反编译 4.1.2 确认）。
    /// 链键取自 refresh token 的 AccessToken（sub / sid claim），与落库行的列同源；App 链没有 sid claim</summary>
    static ValidatedTokenRequest RefreshRequest(string subjectId, string clientId, string? sessionId,
        TokenUsage usage = TokenUsage.OneTimeOnly)
    {
        var claims = new List<Claim> { new("sub", subjectId) };
        if (sessionId != null)
        {
            claims.Add(new Claim("sid", sessionId));
        }

        var refreshToken = new RefreshToken
        {
            CreationTime = DateTime.UtcNow.AddHours(-3),
            Lifetime = 2592000,
            AccessToken = new Token { ClientId = clientId, Claims = claims }
        };

        return new ValidatedTokenRequest
        {
            GrantType = "refresh_token",
            Client = NewClient(clientId, usage),
            RefreshToken = refreshToken,
            RefreshTokenHandle = "handle",
            Subject = refreshToken.Subject
        };
    }

    /// <summary>新登录请求：subject 已由 grant 验证器赋值；SessionId 仅 authorization_code 分支才有值。
    /// <paramref name="requestedOffline"/> = 本次请求是否申请了 offline_access（scope 校验阶段写入
    /// <c>ValidatedResources.Resources.OfflineAccess</c>，null 表示该字段没被赋值）；
    /// 客户端本身默认 <c>AllowOfflineAccess=true</c>（与 AddClientDto 的默认一致）——"被允许"不等于"本次申请了"</summary>
    static ValidatedTokenRequest LoginRequest(string grantType, string subjectId, string clientId,
        string? sessionId = null, bool? requestedOffline = true, bool clientAllowsOffline = true) => new()
    {
        GrantType = grantType,
        Client = NewClient(clientId, offline: clientAllowsOffline),
        Subject = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("sub", subjectId) })),
        SessionId = sessionId,
        ValidatedResources = requestedOffline is null
            ? null!
            : new ResourceValidationResult(new Resources { OfflineAccess = requestedOffline.Value })
    };

    async Task<Dictionary<string, object>> ValidateAsync(ValidatedTokenRequest request, PersistedGrantDbContext? context = null)
    {
        var validator = new CustomTokenRequestValidator(
            context ?? _context, NullLogger<CustomTokenRequestValidator>.Instance);
        var validationContext = new CustomTokenRequestValidationContext
        {
            Result = new TokenRequestValidationResult(request)
        };

        await validator.ValidateAsync(validationContext);

        Assert.False(validationContext.Result.IsError);
        return validationContext.Result.CustomResponse;
    }

    /// <summary>一组固定的"不该被动"的邻居行 + 目标链上的几种行，覆盖 Type / 用户 / 客户端 / 会话各维度</summary>
    GrantEntity[] SeedRowsAroundAppChain() => new[]
    {
        // 目标 App 链 (sub-1, app, sid=null)
        Row("app-consumed-1", "sub-1", "app", null, consumed: true),
        Row("app-consumed-2", "sub-1", "app", null, consumed: true),
        Row("app-alive", "sub-1", "app", null, consumed: false),
        // 邻居：同用户同客户端的 Web 链 / 其他客户端 / 其他用户
        Row("web-consumed", "sub-1", "app", "sid-1", consumed: true),
        Row("web-alive", "sub-1", "app", "sid-1", consumed: false),
        Row("other-client-consumed", "sub-1", "other-app", null, consumed: true),
        Row("other-sub-consumed", "sub-2", "app", null, consumed: true),
        // 邻居：其他 Type（即便 ConsumedTime 有值也不动）
        Row("consent", "sub-1", "app", null, consumed: true, type: "user_consent"),
        Row("auth-code", "sub-1", "app", null, consumed: true, type: "authorization_code"),
        Row("portal", "sub-1", "sso-portal", null, consumed: false, type: "sso_session"),
    };

    #endregion

    #region refresh：修剪已消费行

    [Fact]
    public async Task Refresh_AppChain_PrunesOnlyConsumedRowsOfThatChain()
    {
        await SeedAsync(SeedRowsAroundAppChain());

        var response = await ValidateAsync(RefreshRequest("sub-1", "app", sessionId: null));

        // 只删目标链上的已消费行；存活行必须还在（refresh 不能触发"顶号"清理，否则正在续期的会话自杀）
        Assert.Equal(
            Keys("app-alive", "web-consumed", "web-alive", "other-client-consumed", "other-sub-consumed",
                "consent", "auth-code", "portal"),
            await RemainingKeysAsync());
        // 原有行为不变
        Assert.Equal("v1.0", response["api_version"]);
    }

    [Fact]
    public async Task Refresh_WebChain_PrunesBySessionId_AppChainUntouched()
    {
        await SeedAsync(
            Row("sid1-consumed", "sub-1", "web", "sid-1", consumed: true),
            Row("sid1-alive", "sub-1", "web", "sid-1", consumed: false),
            Row("sid2-consumed", "sub-1", "web", "sid-2", consumed: true),
            // App 链（SessionId 为 null）不能被 Web 链的修剪波及：null 不是"不过滤"
            Row("app-consumed", "sub-1", "web", null, consumed: true));

        await ValidateAsync(RefreshRequest("sub-1", "web", sessionId: "sid-1"));

        Assert.Equal(
            Keys("sid1-alive", "sid2-consumed", "app-consumed"),
            await RemainingKeysAsync());
    }

    [Fact]
    public async Task Refresh_ReUseClient_DoesNotPrune()
    {
        // ReUse 不轮换，正常情况下没有已消费行；即便有，也不是本策略的处理对象
        await SeedAsync(
            Row("app-consumed", "sub-1", "app", null, consumed: true),
            Row("app-alive", "sub-1", "app", null, consumed: false));

        await ValidateAsync(RefreshRequest("sub-1", "app", sessionId: null, usage: TokenUsage.ReUse));

        Assert.Equal(Keys("app-consumed", "app-alive"), await RemainingKeysAsync());
    }

    [Fact]
    public async Task Refresh_KeepsNewestSignalAfterRotation()
    {
        // 模拟一次完整轮换：修剪发生在签发之前；之后 IS4 把当前行标记已消费并插入新行。
        // 链上最终只剩 2 行，且已消费行带着"最近续期时刻"
        await SeedAsync(
            Row("old-consumed-1", "sub-1", "app", null, consumed: true),
            Row("old-consumed-2", "sub-1", "app", null, consumed: true),
            Row("current", "sub-1", "app", null, consumed: false));

        await ValidateAsync(RefreshRequest("sub-1", "app", sessionId: null));

        // 签发阶段（IS4 DefaultRefreshTokenService.UpdateRefreshTokenAsync）：当前行标记已消费 + 新行
        var current = await _context.PersistedGrants.SingleAsync(g => g.Key == "current");
        var rotatedAt = DateTime.UtcNow;
        current.ConsumedTime = rotatedAt;
        _context.PersistedGrants.Add(Row("next", "sub-1", "app", null, consumed: false));
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var rows = await _context.PersistedGrants.AsNoTracking().ToListAsync();
        Assert.Equal(Keys("current", "next"), rows.Select(r => r.Key).OrderBy(k => k, StringComparer.Ordinal).ToList());
        Assert.Equal(rotatedAt, rows.Single(r => r.Key == "current").ConsumedTime);
        Assert.Null(rows.Single(r => r.Key == "next").ConsumedTime);
    }

    #endregion

    #region 新登录：单活 App 会话（原先因实体类型绑错从未生效）

    [Fact]
    public async Task PasswordLogin_RevokesAllPreviousAppRowsOfSubjectAndClient()
    {
        await SeedAsync(SeedRowsAroundAppChain());

        await ValidateAsync(LoginRequest("password", "sub-1", "app"));

        // 同 (sub-1, app) 的 App 链整条（存活 + 已消费）都被清掉；Web 链 / 其他客户端 / 其他用户 / 其他 Type 不动
        Assert.Equal(
            Keys("web-consumed", "web-alive", "other-client-consumed", "other-sub-consumed",
                "consent", "auth-code", "portal"),
            await RemainingKeysAsync());
    }

    [Theory]
    [InlineData("urn:ietf:params:oauth:grant-type:token-exchange", null, true, true)] // 管理员模拟（RFC 8693 token-exchange，旧的 impersonation 扩展授权已在 #1499 删除）：不能杀被模拟者的真实会话
    [InlineData("password", null, false, true)]             // 客户端允许 offline_access，但本次没申请：不会签发新 refresh_token，不能清旧会话
    [InlineData("password", null, null, true)]              // scope 校验结果缺失：按"未申请"处理
    [InlineData("password", null, false, false)]            // 客户端不允许 offline_access：同样不会产生会话行
    [InlineData("authorization_code", "sid-1", true, true)] // 浏览器 code 流（有 sid）：Web 渠道，不在单活范围
    public async Task Login_ExcludedCases_DoNotRevokeAnything(
        string grantType, string? sessionId, bool? requestedOffline, bool clientAllowsOffline)
    {
        await SeedAsync(SeedRowsAroundAppChain());

        await ValidateAsync(LoginRequest(grantType, "sub-1", "app", sessionId, requestedOffline, clientAllowsOffline));

        Assert.Equal(
            Keys("app-consumed-1", "app-consumed-2", "app-alive", "web-consumed", "web-alive",
                "other-client-consumed", "other-sub-consumed", "consent", "auth-code", "portal"),
            await RemainingKeysAsync());
    }

    #endregion

    [Fact]
    public async Task DeleteFailure_IsSwallowed_AndDoesNotBlockTokenRequest()
    {
        // 清理是尽力而为：数据库不可用时只记 Warning，不能让登录 / 刷新失败
        await SeedAsync(Row("app-consumed", "sub-1", "app", null, consumed: true));
        var brokenOptions = new DbContextOptionsBuilder<PersistedGrantDbContext>()
            .UseSqlite(new SqliteConnection("DataSource=:memory:"))
            .Options;
        var broken = new PersistedGrantDbContext(brokenOptions, new OperationalStoreOptions());
        await broken.DisposeAsync();

        var refreshResponse = await ValidateAsync(RefreshRequest("sub-1", "app", sessionId: null), broken);
        var loginResponse = await ValidateAsync(LoginRequest("password", "sub-1", "app"), broken);

        Assert.Equal("v1.0", refreshResponse["api_version"]);
        Assert.Equal("v1.0", loginResponse["api_version"]);
        Assert.Equal(Keys("app-consumed"), await RemainingKeysAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
