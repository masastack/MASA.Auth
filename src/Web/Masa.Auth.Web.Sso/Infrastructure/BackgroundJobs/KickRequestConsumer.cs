// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Subjects;
using StackExchange.Redis;

namespace Masa.Auth.Web.Sso.Infrastructure.BackgroundJobs;

/// <summary>
/// 消费 Admin 的踢人通知（Redis Stream auth:kick-requests，consumer group "sso"），
/// 向被踢会话涉及的客户端发送 OIDC back-channel logout。
///
/// 消息按 <see cref="KickTarget"/> 拆分：每个被踢的浏览器会话（sid）各发一次带 sid 的通知，
/// 满足开启 BackChannelLogoutSessionRequired 的客户端对 sid 的要求；无 sid 的 App 登录合为一个目标，
/// 要求 sid 的客户端由 <see cref="OnlineUser.KickBackChannelLogoutService"/> 跳过并告警。
///
/// 可靠性：
/// - XREADGROUP 消费 → 成功后 XACK；失败不 ACK，留在 pending，由周期性 XAUTOCLAIM
///   （含其他实例宕机遗留的 pending）重试；Redis &lt; 6.2 没有 XAUTOCLAIM 时自动降级为 XPENDING + XCLAIM；
///   单条失败不影响同批其他条目；
/// - 幂等粒度为"消息 × 目标"，键为 online:kick-done:{EntryId}:{目标序号}，**两段式**：
///   先 SETNX 一个短 TTL 的处理租约（<see cref="ProcessingLeaseTtl"/>），发送成功后再改写为 7 天的已完成标记。
///   这样重复投递或部分目标失败后的重试不会向已成功的目标重复发通知；而进程在"抢到标记→发送完成"之间崩溃时，
///   租约会自行过期，消息被 XAUTOCLAIM 重新认领后能重发——若直接写 7 天标记，这个窗口里的通知会永久丢失。
///   租约被他人持有（尚未过期、也不是已完成）的目标本轮不处理、整条消息也不 ACK，留给下次认领；
/// - 超过 <see cref="MaxDeliveryCount"/> 次投递仍失败的消息转入死信 Stream 并 ACK，避免永远重试；
/// - 消费组首次创建从 Stream 起点（0-0）开始：SSO 首次启动前 Admin 已写入的踢人通知也会补发。
///
/// 注意：IS4 的 back-channel HTTP 客户端会吞掉对客户端的 HTTP 失败（只记日志），所以
/// "客户端不可达"不会触发重试；这里的重试只覆盖 SSO 自身宕机/Redis 故障/令牌签发失败等情形。
/// Admin 侧已同步删 grant 行并写水位线，本消费者宕机不阻碍令牌失效，只延迟 backchannel。
/// </summary>
public class KickRequestConsumer : BackgroundService
{
    internal static readonly TimeSpan ClaimIdleThreshold = TimeSpan.FromMinutes(5);

    /// <summary>目标处理租约的 TTL。**必须小于 <see cref="ClaimIdleThreshold"/>**：崩溃实例遗留的消息要空闲满
    /// ClaimIdleThreshold 才会被重新认领，到那时租约必须已经过期，否则认领者会把目标当成"别人正在处理"而再次推迟。
    /// 也要明显长于一次 backchannel 发送的耗时（IS4 的 HTTP 客户端有自己的超时），避免发送过程中租约过期被他人抢走</summary>
    internal static readonly TimeSpan ProcessingLeaseTtl = TimeSpan.FromMinutes(2);

    /// <summary>目标发送成功后的已完成标记 TTL（重复投递去重的窗口）</summary>
    internal static readonly TimeSpan DoneMarkerTtl = TimeSpan.FromDays(7);

    /// <summary>租约值；其它任何值（含旧版本写入的 "1"）都视为"已完成"</summary>
    internal const string LeaseValue = "processing";

    internal const string DoneValue = "done";

    static readonly TimeSpan LoopInterval = TimeSpan.FromSeconds(1);
    static readonly TimeSpan ErrorRetryDelay = TimeSpan.FromSeconds(5);
    const int BatchSize = 10;

    /// <summary>单条消息最大投递次数（含首次）。超过后转入死信 Stream，不再重试</summary>
    const int MaxDeliveryCount = 5;

    const int DeadStreamMaxLength = 1000;

    /// <summary>降级路径（XPENDING）单次扫描的 pending 条数，需大于 <see cref="BatchSize"/></summary>
    const int PendingScanCount = 100;

    /// <summary>Redis 不支持 XAUTOCLAIM（&lt; 6.2）；仅在 ExecuteAsync 的单线程循环内读写</summary>
    bool _autoClaimUnsupported;

    readonly IConnectionMultiplexer _mux;
    readonly IServiceScopeFactory _scopeFactory;
    readonly ILogger<KickRequestConsumer> _logger;
    readonly string _consumerName;

    public KickRequestConsumer(
        IConnectionMultiplexer mux,
        IServiceScopeFactory scopeFactory,
        ILogger<KickRequestConsumer> logger)
    {
        _mux = mux;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _consumerName = $"{Environment.MachineName}:{Guid.NewGuid().ToString("N")[..8]}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _mux.GetDatabase();

        // 消费组的创建/重建全部放进循环、由下面的 catch 统一兜底重试：
        // BackgroundService 里逃出 ExecuteAsync 的异常默认会让整个宿主停机（StopHost），
        // 而 SSO 是登录入口，不能因为踢人通知这条旁路的 Redis 抖动被带走
        var groupReady = false;
        var lastClaimAt = DateTime.MinValue;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!groupReady)
                {
                    await EnsureGroupAsync(db);
                    groupReady = true;
                }

                var entries = await db.StreamReadGroupAsync(
                    OnlineUserConsts.KickRequestStream,
                    OnlineUserConsts.KickConsumerGroup,
                    _consumerName,
                    ">",
                    count: BatchSize);

                foreach (var entry in entries)
                {
                    await HandleEntryAsync(db, entry);
                }

                // 周期性认领 pending（本实例或宕机实例未 ACK 的消息）
                if ((DateTime.UtcNow - lastClaimAt) > ClaimIdleThreshold)
                {
                    lastClaimAt = DateTime.UtcNow;
                    await ClaimPendingAsync(db);
                }

                if (entries.Length == 0)
                {
                    await Task.Delay(LoopInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // 正常停机
            }
            catch (RedisServerException ex) when (ex.Message.Contains("NOGROUP"))
            {
                // 消费组/Stream 被删（Redis 数据被清理、主从切换丢数据等）：只标记，下一轮循环顶部重建。
                // 不能在 catch 块里直接 await EnsureGroupAsync：重建若再失败，异常不会被同级 catch 接住，会逃出循环
                groupReady = false;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "KickRequestConsumer loop error");
                await Task.Delay(ErrorRetryDelay, stoppingToken);
            }
        }
    }

    async Task EnsureGroupAsync(IDatabase db)
    {
        try
        {
            // createStream：流不存在时自动创建；Beginning(0-0)：首次创建消费组时从头消费，
            // 不丢 SSO 首次启动之前已写入的通知（幂等键保证重复处理无副作用）
            await db.StreamCreateConsumerGroupAsync(
                OnlineUserConsts.KickRequestStream,
                OnlineUserConsts.KickConsumerGroup,
                position: StreamPosition.Beginning,
                createStream: true);
            _logger.LogInformation("Kick consumer group created for stream {Stream}", OnlineUserConsts.KickRequestStream);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            // 组已存在
        }
    }

    /// <summary>处理单条消息：全部目标都已落定则 ACK；失败或有目标被他人持有租约时只保留在 pending，
    /// 不影响同批其他条目。返回值 = 是否已 ACK（测试与日志用）</summary>
    internal async Task<bool> HandleEntryAsync(IDatabase db, StreamEntry entry)
    {
        try
        {
            var settled = await ProcessAsync(db, entry);
            if (!settled)
            {
                return false;
            }

            await db.StreamAcknowledgeAsync(
                OnlineUserConsts.KickRequestStream,
                OnlineUserConsts.KickConsumerGroup,
                entry.Id);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to process kick message {EntryId}, left pending for retry", entry.Id);
            return false;
        }
    }

    async Task ClaimPendingAsync(IDatabase db)
    {
        StreamEntry[] claimed;
        if (_autoClaimUnsupported)
        {
            claimed = await ClaimPendingLegacyAsync(db);
        }
        else
        {
            try
            {
                var result = await db.StreamAutoClaimAsync(
                    OnlineUserConsts.KickRequestStream,
                    OnlineUserConsts.KickConsumerGroup,
                    _consumerName,
                    (long)ClaimIdleThreshold.TotalMilliseconds,
                    "0-0",
                    count: BatchSize);
                claimed = result.ClaimedEntries;
            }
            catch (RedisServerException ex) when (ex.Message.Contains("unknown command", StringComparison.OrdinalIgnoreCase))
            {
                // Redis < 6.2 不支持 XAUTOCLAIM：记住结论（之后不再每轮试错），改走 XPENDING + XCLAIM（Redis 5.0+）
                _autoClaimUnsupported = true;
                _logger.LogInformation(
                    "XAUTOCLAIM is not supported (Redis < 6.2), falling back to XPENDING + XCLAIM for pending kick messages");
                claimed = await ClaimPendingLegacyAsync(db);
            }
        }

        foreach (var entry in claimed)
        {
            if (entry.IsNull)
            {
                continue; // 已被 XDEL/MAXLEN 裁剪的条目
            }

            if (await ExceededDeliveryLimitAsync(db, entry.Id))
            {
                await DeadLetterAsync(db, entry);
                continue;
            }

            await HandleEntryAsync(db, entry);
        }
    }

    /// <summary>
    /// XAUTOCLAIM 的 Redis &lt; 6.2 替代实现：XPENDING 列出消费组全部 pending（不限 consumer，含宕机实例遗留），
    /// 客户端侧按空闲时长筛选（XPENDING 的 IDLE 选项同样要求 6.2+，不能用），再 XCLAIM 认领。
    /// XCLAIM 自带 min-idle-time 判定：并发实例同时认领同一条时只有一方成功，不会重复处理。
    /// 扫描窗口大于单批认领数：刚被认领/正在处理的条目空闲时长为 0，会占住窗口前部，不放大窗口会饿死后面真正空闲的条目。
    /// </summary>
    async Task<StreamEntry[]> ClaimPendingLegacyAsync(IDatabase db)
    {
        var minIdleMs = (long)ClaimIdleThreshold.TotalMilliseconds;
        var pending = await db.StreamPendingMessagesAsync(
            OnlineUserConsts.KickRequestStream,
            OnlineUserConsts.KickConsumerGroup,
            PendingScanCount,
            RedisValue.Null);

        var ids = pending
            .Where(p => p.IdleTimeInMilliseconds >= minIdleMs)
            .Take(BatchSize)
            .Select(p => p.MessageId)
            .ToArray();
        if (ids.Length == 0)
        {
            return Array.Empty<StreamEntry>();
        }

        return await db.StreamClaimAsync(
            OnlineUserConsts.KickRequestStream,
            OnlineUserConsts.KickConsumerGroup,
            _consumerName,
            minIdleMs,
            ids);
    }

    async Task<bool> ExceededDeliveryLimitAsync(IDatabase db, RedisValue entryId)
    {
        var pending = await db.StreamPendingMessagesAsync(
            OnlineUserConsts.KickRequestStream,
            OnlineUserConsts.KickConsumerGroup,
            1,
            _consumerName,
            entryId,
            entryId);
        return pending.Length > 0 && pending[0].DeliveryCount > MaxDeliveryCount;
    }

    async Task DeadLetterAsync(IDatabase db, StreamEntry entry)
    {
        var payload = entry.Values.FirstOrDefault(v => v.Name == "message").Value;
        await db.StreamAddAsync(
            OnlineUserConsts.KickRequestDeadStream,
            "message",
            payload.IsNull ? RedisValue.EmptyString : payload,
            maxLength: DeadStreamMaxLength,
            useApproximateMaxLength: true);
        await db.StreamAcknowledgeAsync(
            OnlineUserConsts.KickRequestStream,
            OnlineUserConsts.KickConsumerGroup,
            entry.Id);
        _logger.LogError(
            "Kick message {EntryId} exceeded {MaxDeliveryCount} deliveries, moved to dead stream {DeadStream}",
            entry.Id, MaxDeliveryCount, OnlineUserConsts.KickRequestDeadStream);
    }

    /// <summary>
    /// 处理一条消息的全部目标。返回 true = 所有目标都已落定（已发送 / 已被标记完成 / 无需发送），调用方可以 ACK；
    /// 返回 false = 至少一个目标的处理租约还被别人持有，本轮不 ACK，留待下次认领（那时租约已过期或已完成）；
    /// 任一目标发送失败抛出异常（调用方同样不 ACK）。
    /// </summary>
    async Task<bool> ProcessAsync(IDatabase db, StreamEntry entry)
    {
        var payload = entry.Values.FirstOrDefault(v => v.Name == "message").Value;
        if (payload.IsNull)
        {
            _logger.LogWarning("Kick message {EntryId} has no payload, skipped", entry.Id);
            return true;
        }

        KickRequestMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<KickRequestMessage>((string)payload!);
        }
        catch (JsonException ex)
        {
            // 毒消息：无法解析则放弃（ACK 由调用方执行），避免堵死队列
            _logger.LogError(ex, "Failed to deserialize kick message {EntryId}", entry.Id);
            return true;
        }

        if (message == null || string.IsNullOrEmpty(message.SubjectId) || message.Targets == null)
        {
            _logger.LogWarning("Kick message {EntryId} has empty subject or targets, skipped", entry.Id);
            return true;
        }

        // IBackChannelLogoutService 的依赖链含 Scoped 服务（如 IMasaStackConfig），
        // 不能在单例的后台服务里长期持有，每条消息开一个 scope 现取
        using var scope = _scopeFactory.CreateScope();
        var backChannel = scope.ServiceProvider.GetRequiredService<IBackChannelLogoutService>();

        var allSettled = true;
        for (var i = 0; i < message.Targets.Count; i++)
        {
            var target = message.Targets[i];
            if (target.ClientIds == null || target.ClientIds.Count == 0)
            {
                continue;
            }

            // 幂等（两段式）：SETNX 一个短租约，抢到者才有权发该目标的通知；
            // 发送成功改写为 7 天的已完成标记，发送失败删除租约以允许重试；
            // 进程在两者之间崩溃时，租约到期自动消失，消息被重新认领后可以重发
            var doneKey = $"{OnlineUserConsts.KickProcessedKeyPrefix}{entry.Id}:{i}";
            var leased = await db.StringSetAsync(doneKey, LeaseValue, ProcessingLeaseTtl, When.NotExists);
            if (!leased)
            {
                var current = await db.StringGetAsync(doneKey);
                if (current.HasValue && current != LeaseValue)
                {
                    _logger.LogDebug("Kick message {EntryId} target {Index} already processed, skipped", entry.Id, i);
                }
                else
                {
                    // 租约仍被他人持有（或恰在两次读取之间过期）：不能当作"已完成"而 ACK，
                    // 否则持有者若崩溃，这个目标的通知就丢了。留在 pending，下次认领时再判定
                    allSettled = false;
                    _logger.LogInformation(
                        "Kick message {EntryId} target {Index} is being processed elsewhere, deferred", entry.Id, i);
                }

                continue;
            }

            try
            {
                await backChannel.SendLogoutNotificationsAsync(new LogoutNotificationContext
                {
                    SubjectId = message.SubjectId,
                    SessionId = target.SessionId,
                    ClientIds = target.ClientIds
                });
                await db.StringSetAsync(doneKey, DoneValue, DoneMarkerTtl);
                _logger.LogInformation(
                    "Backchannel logout sent for subject {SubjectId} (session {SessionId}, clients: {ClientIds})",
                    message.SubjectId, target.SessionId ?? "<none>", string.Join(",", target.ClientIds));
            }
            catch (Exception ex)
            {
                await ReleaseLeaseAsync(db, doneKey);
                _logger.LogError(ex,
                    "Failed to send backchannel logout for subject {SubjectId} (session {SessionId})",
                    message.SubjectId, target.SessionId ?? "<none>");
                throw; // 不 ACK，留在 pending 等待重试
            }
        }

        return allSettled;
    }

    /// <summary>释放租约。释放本身失败不能盖掉原始的发送异常（租约有 TTL，最坏是多等一个租约周期）</summary>
    async Task ReleaseLeaseAsync(IDatabase db, string doneKey)
    {
        try
        {
            await db.KeyDeleteAsync(doneKey);
        }
        catch (Exception releaseEx)
        {
            _logger.LogWarning(releaseEx,
                "Failed to release processing lease {Key}; it expires within {Ttl}", doneKey, ProcessingLeaseTtl);
        }
    }
}
