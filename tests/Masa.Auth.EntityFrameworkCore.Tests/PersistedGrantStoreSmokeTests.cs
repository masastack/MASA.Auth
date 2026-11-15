// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Masa.Auth.EntityFrameworkCore.Tests;

/// <summary>
/// grant 冒烟测试（verify-grant-store）：SSO 运行时的 grant store 已静态查明是 IS4 官方 EF
/// operational store（AddOperationalStore&lt;PersistedGrantDbContext&gt;；Masa 的 AddOidcCacheStorage
/// 注册的是 Masa 自己命名空间的同名接口，无法实现 IS4 接口，见计划二节证据链）。
/// 本测试用同一实现 + SQLite 内存库验证在线用户功能依赖的四个语义：
/// 1. 自定义 Type="sso_session"（门户会话行）可经 IS4 的 StoreAsync 写入并被读出——IS4 不校验 Type 枚举；
/// 2. IS4 store 的 RemoveAllAsync(SubjectId) 不区分 Type（含自定义门户行与 user_consent）——这是 IS4 自身语义；
///    Admin 的"踢用户全部"不走这个 API，而是在 AuthDbContext 上排除 user_consent 后删除
///    （见 Masa.Auth.Service.Admin.Tests.OnlineUserSessionTests）；
/// 3. 按 SubjectId+SessionId 删除只删该浏览器会话——"踢整个浏览器"粒度可行；
/// 4. TokenCleanupService 按 Expiration 清理、不分 Type——门户行到期自动消失，无需清理代码。
/// 真实栈上的"踢人删行后 refresh 必须 invalid_grant"需运行环境，作为发布前手工验证项记录在
/// online-user-kick_plan.md。
/// </summary>
public class PersistedGrantStoreSmokeTests : IDisposable
{
    readonly SqliteConnection _connection;
    readonly OperationalStoreOptions _storeOptions;
    readonly PersistedGrantDbContext _context;
    readonly IPersistedGrantStore _store;

    public PersistedGrantStoreSmokeTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _storeOptions = new OperationalStoreOptions();
        var dbOptions = new DbContextOptionsBuilder<PersistedGrantDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new PersistedGrantDbContext(dbOptions, _storeOptions);
        _context.Database.EnsureCreated();
        _store = new PersistedGrantStore(_context, NullLogger<PersistedGrantStore>.Instance);
    }

    static IdentityServer4.Models.PersistedGrant Grant(string type, string key, string? sessionId, DateTime now, DateTime? expiration) => new()
    {
        Key = key,
        Type = type,
        SubjectId = "sub-1",
        ClientId = "sso-portal",
        SessionId = sessionId,
        CreationTime = now,
        Expiration = expiration,
        Data = "{}"
    };

    [Fact]
    public async Task StoreAsync_CustomSsoSessionType_RoundTrips()
    {
        var now = DateTime.UtcNow;
        await _store.StoreAsync(Grant("sso_session", "sso-session:sub-1:sid-1", "sid-1", now, now.AddDays(30)));

        var grants = await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = "sub-1" });

        var portal = grants.Single(g => g.Type == "sso_session");
        Assert.Equal("sso-session:sub-1:sid-1", portal.Key);
        Assert.Equal("sso-portal", portal.ClientId);
        Assert.Equal("sid-1", portal.SessionId);
    }

    [Fact]
    public async Task RemoveAllBySubject_RemovesEveryType_IncludingSsoSession()
    {
        var now = DateTime.UtcNow;
        foreach (var (type, key, sessionId) in new (string Type, string Key, string? SessionId)[]
                 {
                     ("refresh_token", "rt-1", "sid-1"),
                     ("user_consent", "consent-1", null),
                     ("sso_session", "sso-session:sub-1:sid-1", "sid-1")
                 })
        {
            await _store.StoreAsync(Grant(type, key, sessionId, now, now.AddDays(7)));
        }

        // IS4 store 语义：按 SubjectId 全删，不区分 Type。
        // 注意：Admin 踢全部会保留 user_consent，所以不使用该 API，这里只验证 IS4 store 对自定义 Type 行没有特殊处理
        await _store.RemoveAllAsync(new PersistedGrantFilter { SubjectId = "sub-1" });

        Assert.Empty(await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = "sub-1" }));
    }

    [Fact]
    public async Task RemoveAllBySubjectAndSession_RemovesOnlyThatBrowserSession()
    {
        var now = DateTime.UtcNow;
        foreach (var (type, key, sessionId) in new (string Type, string Key, string? SessionId)[]
                 {
                     ("refresh_token", "rt-sid1", "sid-1"),
                     ("sso_session", "sso-session:sub-1:sid-1", "sid-1"),
                     ("refresh_token", "rt-sid2", "sid-2")
                 })
        {
            await _store.StoreAsync(Grant(type, key, sessionId, now, now.AddDays(7)));
        }

        // Admin "踢整个浏览器会话" 语义：SubjectId + SessionId
        await _store.RemoveAllAsync(new PersistedGrantFilter { SubjectId = "sub-1", SessionId = "sid-1" });

        var remaining = await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = "sub-1" });
        var grant = Assert.Single(remaining);
        Assert.Equal("rt-sid2", grant.Key);
    }

    [Fact]
    public async Task TokenCleanup_RemovesExpiredRows_RegardlessOfType()
    {
        var now = DateTime.UtcNow;
        await _store.StoreAsync(Grant("sso_session", "sso-session:sub-1:sid-expired", "sid-expired", now.AddDays(-31), now.AddDays(-1)));
        await _store.StoreAsync(Grant("refresh_token", "rt-valid", "sid-1", now, now.AddDays(7)));

        var cleanup = new TokenCleanupService(
            _storeOptions, _context, NullLogger<TokenCleanupService>.Instance);
        await cleanup.RemoveExpiredGrantsAsync();

        var remaining = await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = "sub-1" });
        var grant = Assert.Single(remaining);
        Assert.Equal("rt-valid", grant.Key);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
