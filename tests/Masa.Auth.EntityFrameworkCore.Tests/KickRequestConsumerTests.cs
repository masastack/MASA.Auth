// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Subjects;
using Masa.Auth.Web.Sso.Infrastructure.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;
using Xunit;

namespace Masa.Auth.EntityFrameworkCore.Tests;

/// <summary>
/// <see cref="KickRequestConsumer"/> 的可靠性语义（用内存里的 Redis 桩，不连真实 Redis）：
/// - 目标处理是两段式幂等：先抢短 TTL 的租约，发送成功再改写为 7 天的已完成标记；
///   进程在"抢到标记→发送完成"之间崩溃时租约自行过期，消息重新认领后能重发（直接写 7 天标记会让这个窗口里的通知永久丢失）；
/// - 租约被别人持有的目标不能被当成"已完成"而 ACK；
/// - 消费组创建/重建失败不能让异常逃出 ExecuteAsync（BackgroundService 默认会因此停掉整个宿主 = SSO 登录入口）。
/// </summary>
public class KickRequestConsumerTests
{
    const string Sub = "sub-1";
    const string EntryId = "1700000000000-0";

    static string DoneKey(int target) => $"{OnlineUserConsts.KickProcessedKeyPrefix}{EntryId}:{target}";

    #region fixtures

    static StreamEntry Entry(params KickTarget[] targets)
    {
        var message = new KickRequestMessage(Sub, targets.ToList(), Guid.NewGuid(), DateTime.UtcNow);
        return new StreamEntry(EntryId, new[] { new NameValueEntry("message", JsonSerializer.Serialize(message)) });
    }

    static KickTarget[] TwoTargets() => new[]
    {
        new KickTarget("sid-1", new List<string> { "webapp" }),   // 浏览器会话
        new KickTarget(null, new List<string> { "app" })          // 无 sid 的 App 登录
    };

    sealed class RecordingBackChannel : IBackChannelLogoutService
    {
        public List<LogoutNotificationContext> Sent { get; } = new();

        /// <summary>发送发生的那一刻被调用（用来观察此刻 Redis 里的状态）</summary>
        public Action<LogoutNotificationContext>? OnSend { get; set; }

        public Func<LogoutNotificationContext, Exception?>? FailWith { get; set; }

        public Task SendLogoutNotificationsAsync(LogoutNotificationContext context)
        {
            OnSend?.Invoke(context);
            var error = FailWith?.Invoke(context);
            if (error != null)
            {
                return Task.FromException(error);
            }

            Sent.Add(context);
            return Task.CompletedTask;
        }
    }

    static (KickRequestConsumer Consumer, RecordingBackChannel BackChannel) NewConsumer(IConnectionMultiplexer? mux = null)
    {
        var backChannel = new RecordingBackChannel();
        var provider = new ServiceCollection()
            .AddSingleton<IBackChannelLogoutService>(backChannel)
            .BuildServiceProvider();
        var consumer = new KickRequestConsumer(
            mux!, provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<KickRequestConsumer>.Instance);
        return (consumer, backChannel);
    }

    #endregion

    [Fact]
    public async Task Success_SendsEveryTarget_MarksThemDoneWithLongTtl_AndAcks()
    {
        var redis = new FakeRedis();
        var (consumer, backChannel) = NewConsumer();

        var acked = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()));

        Assert.True(acked);
        Assert.Equal(new[] { EntryId }, redis.Acked);
        Assert.Equal(2, backChannel.Sent.Count);
        Assert.Equal("sid-1", backChannel.Sent[0].SessionId);
        Assert.Null(backChannel.Sent[1].SessionId);
        // 已完成标记：值 done，TTL 7 天（覆盖了发送前的短租约）
        foreach (var target in new[] { 0, 1 })
        {
            Assert.Equal(KickRequestConsumer.DoneValue, redis.Value(DoneKey(target)));
            Assert.Equal(KickRequestConsumer.DoneMarkerTtl, redis.Ttl(DoneKey(target)));
        }
    }

    [Fact]
    public async Task LeaseIsAcquiredBeforeSending_WithShortTtl()
    {
        var redis = new FakeRedis();
        var (consumer, backChannel) = NewConsumer();
        string? valueSeenDuringSend = null;
        TimeSpan? ttlSeenDuringSend = null;
        backChannel.OnSend = _ =>
        {
            // 发送发生的那一刻，标记必须已经存在，且是短 TTL 的租约，而不是 7 天的完成标记
            valueSeenDuringSend = redis.Value(DoneKey(0));
            ttlSeenDuringSend = redis.Ttl(DoneKey(0));
        };

        await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()[0]));

        Assert.Equal(KickRequestConsumer.LeaseValue, valueSeenDuringSend);
        Assert.Equal(KickRequestConsumer.ProcessingLeaseTtl, ttlSeenDuringSend);
    }

    [Fact]
    public async Task CrashAfterLease_LeaseExpires_ReclaimedMessageIsResent_WithoutDuplicates()
    {
        // 模拟上一个实例抢到目标 0 的租约后崩溃：租约还在，但没有任何通知发出
        var redis = new FakeRedis();
        redis.Seed(DoneKey(0), KickRequestConsumer.LeaseValue, KickRequestConsumer.ProcessingLeaseTtl);
        var (consumer, backChannel) = NewConsumer();

        // 租约未过期：目标 0 本轮不处理，整条消息不能 ACK（否则崩溃者留下的这个目标永远丢了）；目标 1 照常发送
        var first = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()));

        Assert.False(first);
        Assert.Empty(redis.Acked);
        Assert.Single(backChannel.Sent);
        Assert.Null(backChannel.Sent[0].SessionId);

        // 租约过期（XAUTOCLAIM 要空闲满 ClaimIdleThreshold 才会重新认领，那时租约早已过期）
        redis.Expire(DoneKey(0));
        var second = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()));

        Assert.True(second);
        Assert.Equal(new[] { EntryId }, redis.Acked);
        // 目标 0 补发了，目标 1 没有重复发送
        Assert.Equal(2, backChannel.Sent.Count);
        Assert.Equal("sid-1", backChannel.Sent[1].SessionId);
    }

    [Theory]
    [InlineData("done")]
    [InlineData("1")]   // 旧版本写入的完成标记：任何非租约值都视为已完成
    public async Task AlreadyDoneTarget_IsSkipped_AndMessageIsAcked(string existingValue)
    {
        var redis = new FakeRedis();
        redis.Seed(DoneKey(0), existingValue, TimeSpan.FromDays(7));
        redis.Seed(DoneKey(1), existingValue, TimeSpan.FromDays(7));
        var (consumer, backChannel) = NewConsumer();

        var acked = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()));

        Assert.True(acked);
        Assert.Empty(backChannel.Sent);
    }

    [Fact]
    public async Task SendFailure_ReleasesLease_LeavesMessagePending_AndRetrySucceeds()
    {
        var redis = new FakeRedis();
        var (consumer, backChannel) = NewConsumer();
        backChannel.FailWith = _ => new InvalidOperationException("signing key unavailable");

        var acked = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()[0]));

        Assert.False(acked);
        Assert.Empty(redis.Acked);
        Assert.Null(redis.Value(DoneKey(0))); // 租约已释放，重试可以立刻重新抢到

        backChannel.FailWith = null;
        var retried = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()[0]));

        Assert.True(retried);
        Assert.Single(backChannel.Sent);
    }

    [Fact]
    public async Task ReleasingTheLeaseFails_DoesNotMaskSendFailure_AndMessageStaysPending()
    {
        var redis = new FakeRedis { FailOn = "KeyDeleteAsync" };
        var (consumer, backChannel) = NewConsumer();
        backChannel.FailWith = _ => new InvalidOperationException("signing key unavailable");

        var acked = await consumer.HandleEntryAsync(redis.Database, Entry(TwoTargets()[0]));

        Assert.False(acked);
        Assert.Empty(redis.Acked);
        // 租约没能释放：靠 TTL 兜底，仍是短租约而不是 7 天标记
        Assert.Equal(KickRequestConsumer.ProcessingLeaseTtl, redis.Ttl(DoneKey(0)));
    }

    [Fact]
    public void ProcessingLease_MustExpireBeforePendingMessagesAreReclaimed()
    {
        // 崩溃实例遗留的消息要空闲满 ClaimIdleThreshold 才会被重新认领；认领时租约必须已过期，
        // 否则认领者会把目标当成"别人正在处理"再次推迟，崩溃窗口的通知仍然补发不出去
        Assert.True(KickRequestConsumer.ProcessingLeaseTtl < KickRequestConsumer.ClaimIdleThreshold);
        Assert.True(KickRequestConsumer.ProcessingLeaseTtl < KickRequestConsumer.DoneMarkerTtl);
    }

    [Fact]
    public async Task PoisonOrEmptyMessages_AreAcked_SoTheyDoNotBlockTheQueue()
    {
        var redis = new FakeRedis();
        var (consumer, backChannel) = NewConsumer();

        var poison = new StreamEntry("1-0", new[] { new NameValueEntry("message", "{not json") });
        var noPayload = new StreamEntry("2-0", Array.Empty<NameValueEntry>());

        Assert.True(await consumer.HandleEntryAsync(redis.Database, poison));
        Assert.True(await consumer.HandleEntryAsync(redis.Database, noPayload));
        Assert.Empty(backChannel.Sent);
    }

    #region ExecuteAsync：消费组创建失败不能带走宿主

    [Fact]
    public async Task ConsumerGroupCreationFailsAtStartup_DoesNotFaultTheBackgroundService()
    {
        var redis = new FakeRedis { CreateGroupFailures = 1 };
        var (consumer, _) = NewConsumer(redis.Multiplexer);

        await consumer.StartAsync(CancellationToken.None);
        try
        {
            // 旧实现：启动时的 EnsureGroupAsync 在循环外，抛出的异常直接让 ExecuteAsync 失败 → 宿主停机
            await WaitUntilAsync(() => redis.CreateGroupCalls >= 1);
            await Task.Delay(300);

            Assert.False(consumer.ExecuteTask!.IsCompleted, "ExecuteAsync must keep running after a failed group creation");
        }
        finally
        {
            await consumer.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task NoGroupThenRebuildFails_DoesNotEscapeTheLoop()
    {
        // 第 1 次建组成功；第一次读取抛 NOGROUP（组被删）；重建时建组再失败。
        // 旧实现在 catch 块里直接 await EnsureGroupAsync：重建抛出的异常不会被同级 catch 接住，逃出 ExecuteAsync
        var redis = new FakeRedis { NoGroupOnFirstRead = true, CreateGroupFailuresAfterFirst = 1 };
        var (consumer, _) = NewConsumer(redis.Multiplexer);

        await consumer.StartAsync(CancellationToken.None);
        try
        {
            await WaitUntilAsync(() => redis.CreateGroupCalls >= 2);
            await Task.Delay(300);

            Assert.False(consumer.ExecuteTask!.IsCompleted, "ExecuteAsync must keep running after a failed rebuild");
        }
        finally
        {
            await consumer.StopAsync(CancellationToken.None);
        }
    }

    static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 5000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (!condition())
        {
            Assert.True(DateTime.UtcNow < deadline, "condition not reached in time");
            await Task.Delay(20);
        }
    }

    #endregion

    /// <summary>
    /// 内存里的 Redis 桩：只实现消费者用到的几个命令（SET NX + TTL / GET / DEL / XACK / 建组 / 读组）。
    /// 用 DispatchProxy 而不是引入 Mock 库；未实现的命令返回"已完成的空结果"。
    /// </summary>
    public sealed class FakeRedis
    {
        readonly Dictionary<string, (string Value, TimeSpan? Ttl)> _keys = new();

        public FakeRedis()
        {
            Database = DispatchProxy.Create<IDatabase, Handler>();
            ((Handler)(object)Database).Owner = this;
            Multiplexer = DispatchProxy.Create<IConnectionMultiplexer, MuxHandler>();
            ((MuxHandler)(object)Multiplexer).Database = Database;
        }

        public IDatabase Database { get; }

        public IConnectionMultiplexer Multiplexer { get; }

        /// <summary>让指定命令（如 KeyDeleteAsync）抛异常</summary>
        public string? FailOn { get; init; }

        public List<string> Acked { get; } = new();

        public int CreateGroupFailures { get; init; }

        /// <summary>第 1 次建组成功之后，再让接下来的这么多次建组失败</summary>
        public int CreateGroupFailuresAfterFirst { get; init; }

        public bool NoGroupOnFirstRead { get; init; }

        public int CreateGroupCalls;

        int _reads;

        public string? Value(string key) => _keys.TryGetValue(key, out var entry) ? entry.Value : null;

        public TimeSpan? Ttl(string key) => _keys.TryGetValue(key, out var entry) ? entry.Ttl : null;

        public void Seed(string key, string value, TimeSpan? ttl) => _keys[key] = (value, ttl);

        /// <summary>模拟 TTL 到期</summary>
        public void Expire(string key) => _keys.Remove(key);

        // DispatchProxy 的子类不能是 sealed（生成的代理类型要继承它）
        public class Handler : DispatchProxy
        {
            public FakeRedis Owner { get; set; } = null!;

            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
                => Owner.Dispatch(targetMethod!, args ?? Array.Empty<object?>());
        }

        public class MuxHandler : DispatchProxy
        {
            public IDatabase Database { get; set; } = null!;

            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
                => targetMethod!.Name == "GetDatabase" ? Database : DefaultFor(targetMethod.ReturnType);
        }

        object? Dispatch(MethodInfo method, object?[] args)
        {
            if (method.Name == FailOn)
            {
                return Faulted(method.ReturnType, new InvalidOperationException($"simulated redis failure on {method.Name}"));
            }

            switch (method.Name)
            {
                case "StringSetAsync":
                {
                    var key = args.OfType<RedisKey>().Single().ToString();
                    var value = args.OfType<RedisValue>().Single().ToString();
                    var ttl = args.Where(a => a is TimeSpan).Select(a => (TimeSpan?)a).FirstOrDefault();
                    var notExists = args.Any(a => a is When w && w == When.NotExists);
                    if (notExists && _keys.ContainsKey(key))
                    {
                        return Completed(method.ReturnType, false);
                    }

                    _keys[key] = (value, ttl);
                    return Completed(method.ReturnType, true);
                }
                case "StringGetAsync":
                {
                    var key = args.OfType<RedisKey>().Single().ToString();
                    RedisValue value = _keys.TryGetValue(key, out var entry) ? entry.Value : RedisValue.Null;
                    return Completed(method.ReturnType, value);
                }
                case "KeyDeleteAsync":
                {
                    var key = args.OfType<RedisKey>().Single().ToString();
                    return Completed(method.ReturnType, _keys.Remove(key));
                }
                case "StreamAcknowledgeAsync":
                {
                    // (key, group, messageId[, flags])：messageId 是第三个 RedisValue 形参
                    var ids = args.OfType<RedisValue>().ToList();
                    Acked.Add(ids.Last().ToString());
                    return Completed(method.ReturnType, 1L);
                }
                case "StreamCreateConsumerGroupAsync":
                {
                    var call = Interlocked.Increment(ref CreateGroupCalls);
                    var failing = call <= CreateGroupFailures || (call > 1 && call - 1 <= CreateGroupFailuresAfterFirst);
                    return failing
                        ? Faulted(method.ReturnType, new InvalidOperationException("simulated group creation failure"))
                        : Completed(method.ReturnType, true);
                }
                case "StreamReadGroupAsync":
                {
                    if (NoGroupOnFirstRead && Interlocked.Increment(ref _reads) == 1)
                    {
                        return Faulted(method.ReturnType, NoGroupException());
                    }

                    return Completed(method.ReturnType, Array.Empty<StreamEntry>());
                }
                default:
                    return DefaultFor(method.ReturnType);
            }
        }

        static RedisServerException NoGroupException()
            => (RedisServerException)Activator.CreateInstance(
                typeof(RedisServerException),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new object[] { "NOGROUP No such key 'auth:kick-requests' or consumer group 'sso'" },
                null)!;

        static object Completed(Type taskType, object? result)
        {
            var inner = taskType.GetGenericArguments()[0];
            return typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(inner).Invoke(null, new[] { result })!;
        }

        static object Faulted(Type taskType, Exception error)
        {
            if (!taskType.IsGenericType)
            {
                return Task.FromException(error);
            }

            var inner = taskType.GetGenericArguments()[0];
            return typeof(Task).GetMethod(nameof(Task.FromException), 1, new[] { typeof(Exception) })!
                .MakeGenericMethod(inner).Invoke(null, new object[] { error })!;
        }

        static object? DefaultFor(Type type)
        {
            if (type == typeof(void)) return null;
            if (type == typeof(Task)) return Task.CompletedTask;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var inner = type.GetGenericArguments()[0];
                return Completed(type, inner.IsValueType ? Activator.CreateInstance(inner) : null);
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
