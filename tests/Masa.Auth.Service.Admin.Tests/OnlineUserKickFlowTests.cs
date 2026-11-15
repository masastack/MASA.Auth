// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Runtime.CompilerServices;
using System.Text.Json;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Subjects;
using Masa.Auth.Service.Admin.Services;
using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// KickUserAsync 的步骤顺序：快照 → Redis（水位线、Stream）→ 最后删行。
/// DB 与 Redis 不在同一事务里，靠"删行放最后"换取可重试性：Redis 失败时 DB 毫无变化，
/// 重试能用完整的快照重建 sid 水位线与 backchannel 目标；若先删行，重试时快照为空，这些信息就永久丢了。
/// </summary>
public class OnlineUserKickFlowTests : IDisposable
{
    const string Sub = "11111111-1111-1111-1111-111111111111";
    static readonly Guid OperatorId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    static readonly DateTime Now = new(2026, 11, 12, 8, 0, 0, DateTimeKind.Utc);
    const int SeededRowCount = 5;

    readonly SqliteAuthDb _db = new();

    public OnlineUserKickFlowTests()
    {
        _db.Context.Set<PersistedGrant>().AddRange(
            // 浏览器 sid-1：两个客户端 + 门户行
            Grant("rt-a1", "refresh_token", "webapp", "sid-1"),
            Grant("rt-a3", "refresh_token", "webapp2", "sid-1"),
            Grant("sso-session:a:sid-1", OnlineUserConsts.PortalSessionType, "sso-portal", "sid-1"),
            // App（无 sid）
            Grant("rt-a4", "refresh_token", "app1", null),
            // 授权同意：踢人不删
            Grant("consent-a1", OnlineUserConsts.UserConsentType, "webapp", null));
        _db.Context.SaveChanges();
    }

    [Fact]
    public async Task Kick_WritesRedisFirst_AndDeletesRowsLast()
    {
        var redis = new RedisRecorder(_db);

        var result = await KickAsync(new KickOnlineUserModel(Sub), redis);

        // 踢全部：sub 水位线 + Stream 通知
        Assert.Equal(new[] { "StringSetAsync", "StreamAddAsync" }, redis.Calls);
        // 两次 Redis 写入发生时，行都还在
        Assert.All(redis.RowsAtCall, n => Assert.Equal(SeededRowCount, n));
        // 最终只剩 user_consent
        Assert.Equal(1, CountRows());
        var ok = Assert.IsType<Ok<KickOnlineUserResultDto>>(result);
        Assert.Equal(4, ok.Value!.RemovedGrants);
    }

    [Theory]
    [InlineData("StringSetAsync")]
    [InlineData("StreamAddAsync")]
    public async Task Kick_RedisFails_LeavesRowsUntouched_AndRetryRebuildsWatermarkAndTargets(string failingCall)
    {
        var model = new KickOnlineUserModel(Sub, SessionId: "sid-1");

        // 第一次：Redis 在水位线或 Stream 步骤失败 → 异常抛出，DB 毫无变化
        var broken = new RedisRecorder(_db) { FailOn = failingCall };
        await Assert.ThrowsAsync<InvalidOperationException>(() => KickAsync(model, broken));
        Assert.Equal(SeededRowCount, CountRows());

        // 重试：快照仍完整，sid 水位线与通知目标都能重建，且这次删行完成
        var healthy = new RedisRecorder(_db);
        var result = await KickAsync(model, healthy);

        Assert.Equal(new[] { "StringSetAsync", "StreamAddAsync" }, healthy.Calls);
        Assert.Equal(new[] { OnlineUserConsts.KickedAtSessionKey("sid-1") }, healthy.StringSetKeys);

        var message = JsonSerializer.Deserialize<KickRequestMessage>(Assert.Single(healthy.StreamPayloads))!;
        var target = Assert.Single(message.Targets);
        Assert.Equal("sid-1", target.SessionId);
        Assert.Equal(new[] { "webapp", "webapp2" }, target.ClientIds.OrderBy(c => c, StringComparer.Ordinal).ToArray());

        var ok = Assert.IsType<Ok<KickOnlineUserResultDto>>(result);
        Assert.Equal(3, ok.Value!.RemovedGrants); // rt-a1 + rt-a3 + 门户行
        Assert.Equal(2, CountRows()); // rt-a4（App，不属于该浏览器）+ consent
    }

    async Task<IResult> KickAsync(KickOnlineUserModel model, RedisRecorder redis)
    {
        var userContext = StubProxy.Create<IUserContext>((method, _) =>
            method.Name == "GetUserId" ? (true, OperatorId) : (false, null));

        // 权限校验已移到 [MasaAuthorize]（由框架在进入处理方法之前完成），处理方法本身不再查权限，
        // 所以这里的 eventBus 只会被审计段落用到，用默认桩即可
        var eventBus = StubProxy.Create<IEventBus>();

        var database = StubProxy.Create<IDatabase>(redis.Handle);
        var mux = StubProxy.Create<IConnectionMultiplexer>((method, _) =>
            method.Name == "GetDatabase" ? (true, database) : (false, null));

        // ServiceBase 的构造函数要求宿主里有 WebApplication（注册路由用），而 KickUserAsync 不使用任何实例状态，
        // 所以跳过构造函数创建实例。operaterProvider 传 null：审计段落有 try/catch（失败不阻断踢人），这里只关心踢人本身
        var service = (OnlineUserService)RuntimeHelpers.GetUninitializedObject(typeof(OnlineUserService));
        return await service.KickUserAsync(
            model, eventBus, _db.Context, mux, null!, userContext, NullLogger<OnlineUserService>.Instance);
    }

    int CountRows() => _db.Context.Set<PersistedGrant>().AsNoTracking().Count(g => g.SubjectId == Sub);

    /// <summary>记录 Redis 写入顺序，并在每次写入时快照 DB 行数；可指定某一步失败</summary>
    sealed class RedisRecorder
    {
        readonly SqliteAuthDb _db;

        public RedisRecorder(SqliteAuthDb db) => _db = db;

        public string? FailOn { get; init; }
        public List<string> Calls { get; } = new();
        public List<int> RowsAtCall { get; } = new();
        public List<string> StringSetKeys { get; } = new();
        public List<string> StreamPayloads { get; } = new();

        public (bool Handled, object? Result) Handle(System.Reflection.MethodInfo method, object?[]? args)
        {
            if (method.Name is not ("StringSetAsync" or "StreamAddAsync"))
            {
                return (false, null);
            }

            Calls.Add(method.Name);
            RowsAtCall.Add(_db.Context.Set<PersistedGrant>().AsNoTracking().Count(g => g.SubjectId == Sub));

            if (method.Name == FailOn)
            {
                var error = new InvalidOperationException("simulated redis failure");
                return (true, method.ReturnType == typeof(Task<bool>)
                    ? Task.FromException<bool>(error)
                    : Task.FromException<RedisValue>(error));
            }

            if (method.Name == "StringSetAsync")
            {
                StringSetKeys.Add(args![0]!.ToString()!);
            }
            else
            {
                StreamPayloads.Add(args![2]!.ToString()!);
            }
            return (false, null);
        }
    }

    static PersistedGrant Grant(string key, string type, string client, string? sid)
    {
        // Masa 实体属性为 private set，测试里经反射赋值
        var grant = new PersistedGrant();
        Set(grant, nameof(PersistedGrant.Key), key);
        Set(grant, nameof(PersistedGrant.Type), type);
        Set(grant, nameof(PersistedGrant.SubjectId), Sub);
        Set(grant, nameof(PersistedGrant.ClientId), client);
        Set(grant, nameof(PersistedGrant.SessionId), sid);
        Set(grant, nameof(PersistedGrant.CreationTime), Now);
        Set(grant, nameof(PersistedGrant.Expiration), Now.AddDays(10));
        Set(grant, nameof(PersistedGrant.Data), "{}");
        return grant;
    }

    static void Set<T>(T target, string propertyName, object? value) where T : class =>
        typeof(T).GetProperty(propertyName)!.SetValue(target, value);

    public void Dispose() => _db.Dispose();
}
