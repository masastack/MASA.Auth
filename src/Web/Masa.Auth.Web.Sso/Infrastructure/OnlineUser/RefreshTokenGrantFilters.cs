// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
// 为什么必须用别名：Masa 的 OpenIdConnect.Domain 包在**全局命名空间**里有一个同名的 PersistedGrant 实体
// （只映射在 Admin 的 AuthDbContext 里），命名空间内的类型优先于任何 using 导入，所以本项目里裸写
// PersistedGrant 会绑到它，而 SSO 的 PersistedGrantDbContext（IS4 EF store）的模型里没有这个类型——
// Set<PersistedGrant>() 编译通过，运行时抛 "Cannot create a DbSet ... not included in the model"。
// 在 SSO 里查 grant 表必须固定为 IS4 EF 的实体。
using GrantEntity = IdentityServer4.EntityFramework.Entities.PersistedGrant;

namespace Masa.Auth.Web.Sso.Infrastructure.OnlineUser;

/// <summary>
/// SSO 侧对 auth.PersistedGrant 中 refresh_token 行的两个清理谓词（纯查询构造，不执行；
/// 由 <see cref="Validations.CustomTokenRequestValidator"/> 调用 <c>ExecuteDeleteAsync</c>，
/// 测试里直接对真实 EF（SQLite）执行，防止谓词与实体类型悄悄跑偏）。
///
/// 两者互补、触发时机不同：
/// - <see cref="StaleAppSessions"/>：**新登录**时，清掉同一 App 客户端上一次登录留下的整条续期链（单活 App 会话）；
/// - <see cref="ConsumedInChain"/>：**refresh** 时，修剪同一次登录的续期链里更早的已消费行。
/// </summary>
public static class RefreshTokenGrantFilters
{
    /// <summary>
    /// 同一用户在同一 App 客户端上的全部 refresh_token 行（存活与已消费都算）：
    /// <c>SessionId == null</c> 即 token-only 登录（App 渠道），Web（code 流，SessionId 非空）与
    /// sso_session 门户行、user_consent 等其他 Type 都不在结果里。
    /// </summary>
    public static IQueryable<GrantEntity> StaleAppSessions(
        IQueryable<GrantEntity> query, string subjectId, string clientId)
        => query.Where(g => g.SubjectId == subjectId
                            && g.ClientId == clientId
                            && g.Type == OnlineUserConsts.RefreshTokenType
                            && g.SessionId == null);

    /// <summary>
    /// 同一续期链（SubjectId + ClientId + SessionId）上的已消费 refresh_token 行。
    ///
    /// 背景：IS4 在 <c>RefreshTokenUsage=OneTimeOnly</c> 下，每次 refresh 都新插入一行，
    /// 旧行只写 <c>ConsumedTime</c> 而不删除，一直留到 Expiration；IS4 自带的 TokenCleanup 只删已过期行。
    /// 链上行数 = refresh 次数，会随 AT 缩短成倍增长。
    ///
    /// 触发时机：refresh_token 请求。此时当前行还没被标记已消费（IS4 在签发时才写），库里的已消费行都是
    /// 更早的历史——全部删除后，本次轮换结束时链上只剩"新插入的存活行 + 刚被标记已消费的当前行"，
    /// Admin 在线判定依赖的"最近续期时刻"（已消费行的 <c>ConsumedTime</c>）不丢。
    ///
    /// 链的定义与 Admin 列表分组一致：<paramref name="sessionId"/> 为 null 是 App 链（token-only 登录），
    /// 按 IS NULL 精确匹配；否则是 Web 链（按 sid）。两者不能混，null 不是"不过滤"。
    /// 只匹配 <c>Type == refresh_token</c> 且 <c>ConsumedTime != null</c>：存活行、其他 Type、
    /// 其他用户 / 客户端 / 会话的行都不会被选中。
    /// </summary>
    public static IQueryable<GrantEntity> ConsumedInChain(
        IQueryable<GrantEntity> query, string subjectId, string clientId, string? sessionId)
    {
        var chain = query.Where(g => g.Type == OnlineUserConsts.RefreshTokenType
                                     && g.SubjectId == subjectId
                                     && g.ClientId == clientId
                                     && g.ConsumedTime != null);

        return sessionId is null
            ? chain.Where(g => g.SessionId == null)
            : chain.Where(g => g.SessionId == sessionId);
    }
}
