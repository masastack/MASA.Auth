// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

/// <summary>
/// 在线用户（按用户分组，每个用户下挂会话明细）。
/// 会话事实源 = auth.PersistedGrant：客户端会话 = 未过期未消费的 refresh_token 行；
/// 门户会话 = Type 为 sso_session 的自写行（见 OnlineUserConsts）。
/// </summary>
public record OnlineUserDto(
    Guid UserId,
    string Account,
    string DisplayName,
    string Avatar,
    bool IsOnline,
    int SessionCount,
    List<UserSessionDto> Sessions);

/// <summary>
/// 单条会话：Web（SessionId 非空，浏览器 code 流，一个 sid 下可能有多条 ClientId）
/// 或 App（SessionId 为空，token-only 流）。App 会话按 (SubjectId, ClientId) 聚合：
/// 单活策略保证同一客户端只有一条存活行，但历史上可能残留已消费的旧行。
/// </summary>
public record UserSessionDto(
    string? SessionId,
    string ClientId,
    string ClientName,
    ClientTypes ClientType,
    string Channel,
    /// <summary>App 会话对应的 refresh_token 行 Key（踢单条时回传）；Web 会话为空（按 SessionId 踢）</summary>
    string? GrantKey,
    /// <summary>会话起点：组内 MIN(CreationTime)。IS4 轮换续期复用同一个 RefreshToken 对象、
    /// 落库时沿用原 CreationTime，因此同一续期链上所有行的 CreationTime 恒等于登录时刻——
    /// 这个值就是"本次登录时间"，同族内恒定，不是"窗口起点"。</summary>
    DateTime LoginTime,
    /// <summary>最近一次续期时刻：组内 MAX(COALESCE(ConsumedTime, CreationTime))。
    /// IS4 每次 refresh_token 轮换都会给旧行写 ConsumedTime（= 轮换时刻），所以已消费行才是活跃信号；
    /// 从未续期的会话退化为 CreationTime（即登录时刻）。
    /// 注意这不是"最近操作时刻"：误差上界约等于客户端的 AccessTokenLifetime（默认 1 小时）。
    /// 纯门户行（sso_session）只有登录时刻，不随门户活动刷新。</summary>
    DateTime LastRenewedAt);

/// <summary>
/// 踢人请求，四级粒度：
/// GrantKey 非空 = 踢单个 App 登录（单条 refresh_token 行）；
/// SessionId+ClientId = 踢该浏览器会话下的某客户端（不影响该浏览器的门户登录与其他客户端）；
/// 仅 SessionId = 踢整个浏览器会话（含门户登录）；
/// GrantKey/SessionId/ClientId 全部为空 = 踢该用户全部会话（不含 user_consent：授权同意不随踢人撤销）。
/// ClientId 必须与 SessionId 一起出现，单独传 ClientId 会被拒绝（400），不会退化为踢全部；
/// GrantKey 已唯一确定一行，同时传 SessionId 同样返回 400（语义冲突）。
/// </summary>
public record KickOnlineUserModel(
    string SubjectId,
    string? SessionId = null,
    string? ClientId = null,
    string? GrantKey = null);

public class GetOnlineUsersDto : Pagination<GetOnlineUsersDto>
{
    public string? Search { get; set; }

    /// <summary>渠道过滤：Web / App / null=全部。过滤同时作用于会话明细：
    /// 返回用户的 Sessions / SessionCount / IsOnline 只统计该渠道的会话，没有该渠道会话的用户不返回</summary>
    public string? Channel { get; set; }

    /// <summary>只返回在线用户（默认 true）。在线判定 = 会话最近续期时刻晚于 now - 阈值。
    /// 关掉后返回所有"仍有有效会话"的用户（含已超阈值、IsOnline=false 的闲置会话），
    /// 用于排查"谁还挂着会话"这类场景。注意 refresh_token 的绝对寿命默认 30 天，
    /// 关掉后列表规模可能远大于在线人数。</summary>
    public bool OnlyOnline { get; set; } = true;
}

/// <summary>
/// Admin → SSO 的踢人通知载荷（Redis Stream 消息体）。
/// 通知按 <see cref="KickTarget"/> 拆分：每个被踢的浏览器会话（sid）各发一次 backchannel，
/// 让 logout_token 携带 sid——开启 BackChannelLogoutSessionRequired 的客户端（IS4 默认开启）
/// 要求必须有 sid，否则 IS4 会抛 "Client requires SessionId"。
/// </summary>
public record KickRequestMessage(
    string SubjectId,
    List<KickTarget> Targets,
    Guid OperatorId,
    DateTime KickedAtUtc);

/// <summary>
/// 一次 backchannel 通知的目标。SessionId 非空 = 某个浏览器会话下涉及的客户端；
/// SessionId 为空 = 无浏览器会话的 App 登录（token-only 流）涉及的客户端，logout_token 仅带 sub，
/// 要求 sid 的客户端会被跳过（无法满足其协议要求）。
/// </summary>
public record KickTarget(string? SessionId, List<string> ClientIds);

/// <summary>踢人结果</summary>
public record KickOnlineUserResultDto(
    /// <summary>删除的 grant **行数**（含已消费的轮换旧行与 authorization_code 等非会话行，
    /// 不含 user_consent）。不等于界面上的会话数，为 0 也不代表失败——水位线与 backchannel 通知仍已生效。</summary>
    int RemovedGrants,
    List<string> ClientIds);
