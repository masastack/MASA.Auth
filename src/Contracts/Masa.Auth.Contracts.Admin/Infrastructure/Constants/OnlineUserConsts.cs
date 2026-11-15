// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

/// <summary>
/// 在线用户 / 踢人功能的共享约定（字符串与数值常量，外加几个由前缀拼键的纯函数；Contracts 层保持零重依赖）。
/// PersistedGrant Type 注册表：IS4 协议端点往 auth.PersistedGrant 写 refresh_token / authorization_code /
/// reference_token / user_consent 四类（IS4 另有 device_code / user_code 两个类型常量，但设备授权流的
/// code 存在独立的 auth.DeviceFlowCodes 表，不会出现在 PersistedGrant 里）；
/// 本项目另外自写 <see cref="PortalSessionType"/>（SSO 门户会话行）。
/// 查询 auth.PersistedGrant 的新代码必须认识它。
/// </summary>
public static class OnlineUserConsts
{
    /// <summary>IS4 内置 grant 类型：refresh_token（客户端会话的事实源）</summary>
    public const string RefreshTokenType = "refresh_token";

    /// <summary>IS4 内置 grant 类型：user_consent（用户"记住授权"记录）。
    /// 踢人只终止会话，不撤销授权同意，因此"踢全部"的过滤要排除这一类</summary>
    public const string UserConsentType = "user_consent";

    /// <summary>门户会话行在 PersistedGrant 中的 Type（IS4 的 Type 列为自由字符串）</summary>
    public const string PortalSessionType = "sso_session";

    /// <summary>门户会话行 Key 前缀，完整 Key = {prefix}{subjectId}:{sid}</summary>
    public const string PortalSessionKeyPrefix = "sso-session:";

    /// <summary>SSO Cookie 的最长寿命（天）。这是"记住我"登录时长的唯一来源：
    /// LoginOptions.RememberMeLoginDuration、门户会话行 TTL、踢人水位线 TTL 都引用它，
    /// 三者必须一致——水位线若比 Cookie 先过期，被踢的 Cookie 会重新生效</summary>
    public const int SsoCookieMaxLifetimeDays = 30;

    /// <summary>门户会话行 Expiration：取 Cookie 最长寿命；
    /// 展示正确性由在线阈值过滤保证，TTL 仅是行卫生兜底</summary>
    public const int PortalSessionLifetimeDays = SsoCookieMaxLifetimeDays;

    /// <summary>Admin → SSO 踢人通知 Stream（传输用途，非存储）</summary>
    public const string KickRequestStream = "auth:kick-requests";

    /// <summary>踢人 Stream 的近似最大长度（XADD MAXLEN ~）。消息消费后不会自动删除，
    /// 不裁剪会无限增长；踢人是低频操作，1 万条足够覆盖任何积压与排查窗口</summary>
    public const int KickRequestStreamMaxLength = 10000;

    /// <summary>踢人通知超过最大投递次数后转入的死信 Stream（仅留档排查，不再消费）</summary>
    public const string KickRequestDeadStream = "auth:kick-requests:dead";

    /// <summary>踢人 Stream 的 consumer group 名</summary>
    public const string KickConsumerGroup = "sso";

    /// <summary>SSO Cookie 拦截水位线键前缀，值为 unix 毫秒，不删键。
    /// 完整键：{prefix}sub:{subjectId}（踢全部）/ {prefix}sid:{sessionId}（踢单会话）</summary>
    public const string KickedAtKeyPrefix = "online:kicked-at:";

    public static string KickedAtSubjectKey(string subjectId) => $"{KickedAtKeyPrefix}sub:{subjectId}";

    public static string KickedAtSessionKey(string sessionId) => $"{KickedAtKeyPrefix}sid:{sessionId}";

    /// <summary>踢人消息已处理标记前缀（幂等去重，值 = Stream EntryId）</summary>
    public const string KickProcessedKeyPrefix = "online:kick-done:";

    /// <summary>水位线键 TTL：取 Cookie 最长寿命，保证水位线不会早于被踢的 Cookie 过期</summary>
    public const int KickedAtKeyTtlDays = SsoCookieMaxLifetimeDays;

    /// <summary>在线阈值默认值（分钟）。语义：会话最近**续期**时刻距今超过该阈值视为离线。
    /// 120 = 2 × IS4 Client 默认的 AccessTokenLifetime（3600s）。AccessTokenLifetime 是逐客户端配置的，
    /// 客户端调大后需通过配置 OnlineUser:OnlineThresholdMinutes 同步调整阈值。
    ///
    /// "最近续期时刻"的来源是 refresh_token 已消费旧行的 ConsumedTime（每次轮换写一次）。
    /// 注意它不是"最近操作时刻"：误差上界约等于客户端的 AccessTokenLifetime。
    /// 两类行没有续期信号，只能退化为登录时刻：
    /// - 客户端配置 RefreshTokenUsage=ReUse（不轮换，不产生 ConsumedTime）；
    /// - 门户行（sso_session）的 CreationTime 只在登录时写一次，不随门户活动刷新。
    /// 详见 online-user-kick_plan.md 的已知限制。</summary>
    public const int DefaultOnlineThresholdMinutes = 120;

    /// <summary>渠道：浏览器（code 流，PersistedGrant.SessionId 非空）</summary>
    public const string ChannelWeb = "Web";

    /// <summary>渠道：App（token-only 流：password/phone_code/email_code/ldap/third_party 等，SessionId 为空）。
    /// client_credentials 没有用户且 IS4 不为它签发 refresh_token，不会出现在在线列表里</summary>
    public const string ChannelApp = "App";
}
