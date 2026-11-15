# 在线用户与踢人（IS4 grant-first 定稿）

> 状态：代码已实现、单元/集成测试通过；**真实环境端到端冒烟尚未执行**（见第 5 节），合并/发布前必须完成
> 定稿日期：2026-11-12（含评审修订：backchannel 脱离 HttpContext、按 sid 扇出、踢全部保留 user_consent、403 语义）
> 修订：2026-11-14（在线判定改用 `ConsumedTime` 聚合、`LastActiveAt`→`LastRenewedAt`、列表 `OnlyOnline`、
> `user_consent` 全分支保留、`GrantKey+SessionId` 400、水位线合并 MGET、App 实时性分层 SOP §7 与阈值联动告警、
> Reference Token + introspection 接入 §8）
> 修订：2026-11-14（App 踢人"分钟级收敛"：新增 refresh 时修剪已消费行、App 接入约定（§7）、客户端 AT 配置；
> **修复单活 App 会话策略因实体类型绑错而从未生效的问题**，见 §1；App 实时性措辞改为"AT 寿命内收敛"）
> 前版方案（匿名 HTTP 端点 + Redis 消息 + kicked 标记）已被本文取代，旧内容见 git 历史。

---

## 1. 设计定稿（v2 grant-first）

**会话事实源 = `auth.PersistedGrant` 表**（IS4 EF operational store，运行时唯一 grant store；
Masa 的 `AddOidcCacheStorage` 注册的是 Masa 自己命名空间的同名接口，不参与 IS4 运行时）。
Keycloak userSession/clientSession 的 IS4 等价物：

- **clientSession** = 一条未过期、未消费的 `refresh_token` 行；
- **userSession**（浏览器维度）= 按 `SessionId` 分组；
- **门户会话** = SSO 自写的 `Type="sso_session"` 行（见 Type 注册表）；
- **user** = `SubjectId` 分组。

IS4 的两个免费特性支撑设计：① `RefreshTokenUsage=OneTimeOnly` 轮换续期——每次续期
INSERT 新行、旧行标 `ConsumedTime`，**已消费旧行的 `ConsumedTime` 即"最近续期时刻"，无需心跳**；
② 滑窗 `Expiration` + `EnableTokenCleanup`——过期行自动清理，列表查询过滤即可。

> 更正（2026-11-14 评审）：早期版本写的是"行 `CreationTime` 即最近活跃时刻"，**这是错的**。
> IS4 轮换时复用同一个 `RefreshToken` 对象、落库沿用原 `CreationTime`，因此同族所有行的
> `CreationTime` 恒等于登录时刻。活跃信号只能从 `ConsumedTime` 拿，详见下文"在线判定"。

### Type 注册表（约定，读这张表的新代码必须认识）

| Type | 写入方 | 说明 |
|---|---|---|
| `refresh_token` / `authorization_code` / `reference_token` / `user_consent` | IS4 协议端点 | IS4 往 `PersistedGrant` 写的四类（IS4 另有 `device_code` / `user_code` 两个类型常量，但设备流 code 存在独立的 `DeviceFlowCodes` 表，不进此表）。**只有 `refresh_token` 行代表可通知的客户端会话**；`user_consent` 不是会话，踢人不删 |
| `sso_session` | SSO `IdentityServerEventSink`（UserLoginSuccess 时 StoreAsync，天然按 Key upsert） | 门户会话行：Key=`sso-session:{sub}:{sid}`、ClientId 占位值 `sso-portal`（ClientId 列非空，门户行不对应真实客户端）、Expiration=30 天（取 `SsoCookieMaxLifetimeDays`，与 `LoginOptions.RememberMeLoginDuration`、水位线 TTL 同源；展示正确性由在线阈值保证） |

常量统一在 `Masa.Auth.Contracts.Admin.Infrastructure.Constants.OnlineUserConsts`。

### 在线判定

**会话最近续期时刻**（组内 `MAX(COALESCE(ConsumedTime, CreationTime))`）距今 ≤ 阈值即在线；阈值默认
120 分钟（= 2 × IS4 默认 `AccessTokenLifetime` 3600s；`AccessTokenLifetime` 是逐客户端配置的，
客户端调大后需用 DCC 配置 `OnlineUser:OnlineThresholdMinutes` 同步覆盖）。
App 会话（SessionId 为空的 token-only 流：password/phone_code/email_code/ldap/third_party；
`client_credentials` 不签发 refresh_token，不会出现在列表里）按 (subjectId, clientId) 组；Web 会话按 (sid, clientId) 组。

#### 为什么不是 `MAX(CreationTime)`（IS4 4.1.2 行为依据）

早期设计写的是"`RefreshTokenUsage=OneTimeOnly` 轮换续期 → 每次 INSERT 新行 → 行 `CreationTime` 即最近活跃时刻"，
**这个结论是错的**，源码链路（`DefaultRefreshTokenService` / `DefaultRefreshTokenStore` / `DefaultGrantStore`）：

1. 轮换时**复用同一个 `RefreshToken` 对象**：只写 `ConsumedTime`、改 `Lifetime`，然后清空 `ConsumedTime` 重新存储；
2. `StoreRefreshTokenAsync` → `CreateItemAsync(..., refreshToken.CreationTime, refreshToken.Lifetime)`——
   `CreationTime` 是从 `Data` 反序列化出来的**原值**。

因此**同一条续期链上所有行的 `CreationTime` 恒等于登录时刻**：`MAX(CreationTime)` 的语义是"最近一次登录"，
不是"最近活跃"。真实活跃信号在**已消费的旧行**上——每次轮换都给旧行写 `ConsumedTime = now`。

据此：
- **会话存活** = 组内存在 `ConsumedTime == null` 且未过期的行（整组都已消费说明续期链终结，不再展示）；
- **最近续期** = 组内 `MAX(COALESCE(ConsumedTime, CreationTime))`，**包含已消费行**；
- 从未续期的会话退化为 `CreationTime`（= 登录时刻），这是唯一可用的近似。

契约字段：`UserSessionDto.LoginTime`（组内 `MIN(CreationTime)`，同族恒定 = 登录时刻）与
`UserSessionDto.LastRenewedAt`（最近续期时刻）。二者在"一次登录 + 多次续期"的会话里**必然不同**。


### Redis 角色（收缩后）

| 键 | 状态 | 用途 |
|---|---|---|
| `online:users` / `online:sso-session:{sub}` | **移除** | 由 `sso_session` 表行取代（旧标记每用户单值、多浏览器覆盖） |
| `kicked:{sub}` | **改造** | → `online:kicked-at:sub:{sub}` / `online:kicked-at:sid:{sid}` 水位线（unix 毫秒，TTL 30 天，**命中不删键**） |
| `auth:kick-requests` | 新增 | Admin→SSO 踢人通知 Stream（consumer group `sso`，MAXLEN≈10000），仅传输 |
| `auth:kick-requests:dead` | 新增 | 投递超过 5 次仍失败的消息（死信，MAXLEN≈1000），需人工排查 |
| `online:kick-done:{entryId}:{目标序号}` | 新增 | 消费幂等标记，两段式：SETNX 抢 2 分钟处理租约（值 `processing`）→ 发送成功改写为 7 天已完成标记（值 `done`） |

### 单活 App 会话策略（token-only 登录清理同客户端旧 refresh_token）

**动机**：token-only 流（password / phone_code / email_code / local_phone / third_party_idp / ldap 等直连
/connect/token 的授权，无浏览器会话）每次完整登录都 INSERT 一条新 refresh_token 行，IS4 没有
"新登录顶替旧会话"机制；客户端若反复登录而不用 refresh_token 续期（实测 pinbar-app 单账号数日内堆积
~400 行），在线列表会按行展开成同 clientId 的海量"会话"。策略：**token-only 登录成功时删除同 (SubjectId, ClientId) 的旧
refresh_token 行，无条件生效、无配置开关**。

**实现**：挂在 SSO `CustomTokenRequestValidator`（IS4 每条 /connect/token 只注入这一个
`ICustomTokenRequestValidator`，扩展必须改本类，不能注册第二个）。IS4
`TokenRequestValidator.RunValidationAsync`（反编译 4.1.2 确认）先跑 grant 校验——password /
扩展 grant 在此完成用户验证并赋值 Subject——**之后**才调用本验证器，令牌签发（新行 INSERT）又在其后，
因此这里 subject 已知且不会误删本次新行。

**token-only 判定是语义的**（不维护 grant 名单）："本次签发的 refresh_token 行会不会是
`SessionId == null`"，判据 = 请求上的 `ValidatedRequest.SessionId` 为空。反编译确认的行为链：
`ValidatedRequest.SessionId` 仅在 authorization_code 分支被赋值（来自浏览器授权时的 code grant，
password / 扩展 grant / refresh_token / client_credentials 分支恒为 null），且它是落库行 SessionId 的
唯一来源（ValidatedRequest.SessionId → sid claim → `Token.SessionId` → `RefreshToken.SessionId` →
`PersistedGrant.SessionId`）。在此读它即是对未来那行的精确预测，与 Admin 列表 `ResolveChannel`
（SessionId==null → "App"）**同源**；以后新增扩展授权验证器无需维护名单，自动纳入单活语义。
除上述判据外再叠加"**本次请求确实会签发 refresh_token**"——即请求申请了 `offline_access`
（`ValidatedRequest.ValidatedResources.Resources.OfflineAccess`，scope 校验阶段赋值、先于本验证器；取不到按未申请处理）——
与 subjectId 非空。**不能用 `Client.AllowOfflineAccess`**：那只是客户端"被允许"（新建客户端默认 true），
一次只申请 `openid profile` 的登录不产生新的 refresh_token 行，若照样清理，会在没有任何替代会话的情况下
杀掉用户在另一台设备上的会话（2026-11-14 评审修正）。删除谓词限定 `Type == refresh_token && SessionId == null`（`ExecuteDeleteAsync`，
与踢人同模式）：Web（code 流）会话与 `sso_session` 门户行不受影响。

**显式排除（仅两个，`SingleActiveAppSessionPolicyTests` 锚定其常量字面量防漂移）**：

- `refresh_token`：轮换续期而非新登录；轮换请求的 `ValidatedRequest.SessionId` 同样为空，不显式排除
  会在每次刷新时把正在续期的会话杀死；
- `impersonation`：管理员模拟用户（扩展授权、无浏览器锚点，语义上无法与普通 token-only 区分），
  不能杀被模拟者的真实会话。

**边界**：

- `device_code` 流的 `ValidatedRequest.SessionId` 同样不赋值，属清理范围；本 SSO 未启用设备流，无实际影响；
- 被顶掉的旧设备下次 refresh 得 `invalid_grant`（标准 OIDC 行为，与踢人后语义一致）；
- 存量堆积行无需一次性清理：同 (sub, client) 下次登录即全删；不再登录的用户最迟按
  refresh_token 过期 + TokenCleanup 自然消失；
- 两个并发登录可能各留一行（≤2 行），下次登录收敛，不加锁；
- 清理失败仅记 Warning 不阻断登录（最坏退化为旧行堆积，下次登录重试）；
- 判定依赖 IS4 内部行为链（4.1.2 反编译逐环确认），升级 IS4 时需复核该链仍成立。

**冒烟**：同账号连续登录两次 → 列表只剩 1 条 App 行且旧行 refresh 返回 `invalid_grant`；
换客户端登录互不影响；浏览器多窗口登录不受策略影响。

> **更正（2026-11-14）：此前的实现从未生效。** 旧代码写的是 `Set<PersistedGrant>().Where(...).ExecuteDeleteAsync()`。
> Masa 的 `OpenIdConnect.Domain` 包在**全局命名空间**里有一个同名的 `PersistedGrant` 实体（只映射在 Admin 的
> `AuthDbContext`），而命名空间内的类型优先于 `using` 导入，所以 SSO 里裸写 `PersistedGrant` 绑到的是它，
> 不是 IS4 EF 的 `IdentityServer4.EntityFramework.Entities.PersistedGrant`。SSO 的 `PersistedGrantDbContext` 模型里没有这个类型：
> 编译通过，运行时抛 `Cannot create a DbSet for 'PersistedGrant' because this type is not included in the model ...`
> （已用真实 `PersistedGrantDbContext` 复现），被 `catch` 记成 Warning
> （`Single-active App session policy: failed to revoke stale refresh_token grants ...`）后吞掉，登录照常成功，
> 所以表现为"旧行一直堆积"而不是报错。
>
> 现已改为经 `RefreshTokenGrantFilters.StaleAppSessions`（`Infrastructure/OnlineUser/`，用别名固定 IS4 EF 实体）
> 查询，并由 `CustomTokenRequestValidatorGrantCleanupTests` 对真实 IS4 `PersistedGrantDbContext`（SQLite）执行覆盖
> ——再绑错类型，删除不会发生，断言立刻失败。
>
> **影响范围**：单活策略连同本分支的其余改动都还没提交（`HEAD` 里的 `CustomTokenRequestValidator` 没有这段逻辑），
> 所以**没有已发布版本受影响，也没有"升级行为变化"**；受影响的只是用本分支工作区构建做过联调的环境——
> 那里"登录后旧行没被清掉"的现象就是这个 bug，此前基于该构建得出的单活策略联调结论作废，须重新冒烟（见上）。
> 排查线索：旧构建日志里持续出现上述 Warning；修复后同一位置变为 Information 级的
> `revoked N stale refresh_token grant(s)`。
>
> 策略本身的既定语义不变，上线前仍需与 App 侧确认可接受：同一账号在**同一 App 客户端**上多设备并行登录会变成
> "后登录者顶掉先登录者"（被顶的设备下次 refresh 得 `invalid_grant`）。

### refresh 时修剪已消费行（与单活策略互补）

**问题**：`RefreshTokenUsage=OneTimeOnly` 下，每次 refresh 都**新插入**一行 `refresh_token`，旧行只被标记 `ConsumedTime`、
**不删除**，一直留到 `Expiration`；IS4 自带的 `TokenCleanup` 只删已过期行。一个活跃会话的行数 = 它的 refresh 次数，
AT 越短增长越快（估算：AT 1 小时约 24 行/天，AT 15 分钟约 96 行/天，30 天可达数千行；每行 `Data` 里还有序列化的令牌声明，通常 1KB 以上）。
影响的是在线列表：`BuildOnlineUsersAsync` 对在线用户名下**所有**行（含已消费历史）读入内存分组，开销随历史行线性增长。
**清理不影响踢人本身的正确性**，它是为了让"缩短 App 的 AT"不把表和列表接口拖垮。

**与单活策略的分工**（触发时机不同，互不覆盖）：

| | 单活 App 会话（`StaleAppSessions`） | refresh 修剪（`ConsumedInChain`） |
|---|---|---|
| 触发 | **新登录**（password 等；`refresh_token` 请求直接跳过） | **`refresh_token` 请求**（仅 `RefreshTokenUsage=OneTimeOnly` 的客户端） |
| 删除范围 | 同 (SubjectId, ClientId)、`SessionId IS NULL` 的**全部**旧行（存活 + 已消费） | 同一续期链 (SubjectId, ClientId, SessionId) 上 `ConsumedTime != null` 的行 |
| 作用 | 顶号：连根清掉上一次登录的整条链 | 同一次登录内修剪历史：链上只留最新的 |
| 覆盖不到的 | 登录一次后长期只靠 refresh 续期的链（移动端常态）；Web 链（`SessionId` 非空） | — |

**实现**：同在 `CustomTokenRequestValidator`（`PruneConsumedRefreshTokensAsync`，`ExecuteDeleteAsync`，不新增 `BackgroundService`，
没有多实例并发问题）；谓词在 `RefreshTokenGrantFilters.ConsumedInChain`。反编译 IS4 4.1.2 确认的时序：
`ValidateRefreshTokenRequestAsync` 已赋值 `ValidatedRequest.RefreshToken` / `Subject`，**之后**才调用自定义验证器；
"当前行标记已消费 + 插入新行"发生在更后面的令牌签发（`DefaultRefreshTokenService.UpdateRefreshTokenAsync`）。
所以此刻当前行仍存活、库里已消费的全是更早的历史，全部删掉即可——**本次轮换结束后链上只剩"新插入的存活行 +
刚被标记已消费的当前行"，稳态每条链 2 行**，在线判定依赖的"最近续期时刻"（已消费行的 `ConsumedTime`）不丢。

- 链键取自 refresh token 本身（`RefreshToken.SubjectId / ClientId / SessionId`，即落库行的同名列），不取请求上的 `SessionId`（refresh 请求的该值恒为空）；
- `SessionId` 为空（App 链）按 `IS NULL` 精确匹配，不是"不过滤"——Web 链的修剪不会波及同用户同客户端的 App 链，反之亦然；
- 失败只记 Warning（`Failed to prune consumed refresh_token rows ...`），不阻断刷新；最坏是旧行多留一轮，下次 refresh 再清。
  若删除后令牌签发失败，该会话的"最近续期"暂时退化为登录时刻，下次成功 refresh 即恢复；
- 存量历史行：每条链在下次 refresh 时一次性清掉；不再 refresh 的会话最迟随 `Expiration` 被 `TokenCleanup` 删除；
- **安全性不变**：IS4 4.1.2 没有 refresh token 重用检测（`AcceptConsumedTokenAsync` 默认拒绝，仓库内无自定义 `IRefreshTokenService`），
  已消费的旧 token 被重放时，无论是"已消费"还是"不存在"，结果都是 `invalid_grant`。**依赖 IS4 内部行为链，升级 IS4 时需复核**：
  ① 自定义验证器调用时 `RefreshToken` 已赋值；② 当前行在签发阶段才被标记已消费；③ 无重用检测。

## 2. 踢人链路（Admin 编排，四级粒度）

请求模型 `KickOnlineUserModel(SubjectId, SessionId?, ClientId?, GrantKey?)`，合法组合：

| 组合 | 含义 | 水位线 |
|---|---|---|
| `GrantKey` | 单个登录（App 会话行 / 单条 grant） | 仅当删除的是门户行时写该 sid |
| `SessionId + ClientId` | 浏览器会话中的单个客户端 | 不写（不拒绝门户 Cookie） |
| `SessionId` | 整个浏览器会话 | 被删行涉及的 sid |
| 全空 | 该用户**全部会话**（**不含 `user_consent`**） | 无条件写 `sub` 水位线（即便没匹配到任何行） |
| 仅 `ClientId` | **非法**，返回 400（过滤层兜底为"匹配 0 行"，绝不退化为全踢） | — |
| `GrantKey + SessionId` | **非法**，返回 400（GrantKey 已唯一确定一行，语义冲突） | — |

`user_consent` 在**所有分支**都被排除（不只是"全部"分支）：踢人只终止会话，不撤销用户的授权同意。
`FilterForKick` 把 `Type != user_consent` 提到最外层的公共谓词上，新增分支自动继承。

3. **写水位线**（同步，不依赖 SSO 在线）：SSO 的 `OnValidatePrincipal` 比较 Cookie `IssuedUtc`
   与水位线（sid / sub 一次 MGET 取回后逐个比对），早于水位线即拒绝——**不删键**，被踢后重新登录（IssuedUtc 更晚）自然通过。
（`IssuedUtc` 随 Cookie 滑动续期更新，但校验发生在续期之前，所以判定成立。）
Cookie 缺 `IssuedUtc`（正常不会出现）按"最早"处理：存在水位线即拒绝（fail-closed）。
   钩子由 `AddSsoKickWatermark()` 注册（`SsoKickWatermarkExtensions`）：必须在 `PostConfigure` 里追加
   并保留原有 `OnValidatePrincipal`，否则会覆盖 IS4 对门户 Cookie 的配置。

1. **鉴权**：方法上的 `[MasaAuthorize]` 校验 `api.online-user.kick`（与 TeamAdd 同一套机制），未授权返回 **403**（见第 3 节）。
2. **快照**：先按过滤条件查出将被删的行（用于 sid 水位线、通知目标）。
   **步骤顺序是刻意的：快照 → 水位线 → Stream → 最后删行。** DB 与 Redis 无法同事务，靠顺序换可重试性：
   Redis 两步都是幂等的，失败发生在删行之前时 DB 毫无变化，管理员重试即可完整重做；若先删行，
   Redis 一旦失败，行已删、重试时快照为空，sid 水位线与 backchannel 目标就永久丢了（`OnlineUserKickFlowTests` 覆盖）。
   删行失败（极少见）则 Redis 侧已生效，重试会补删并重发通知（客户端处理重复 logout_token 天然幂等）。
3. **写水位线**（同步，不依赖 SSO 在线）：SSO 的 `OnValidatePrincipal` 比较 Cookie `IssuedUtc`
   与水位线（先 sid 后 sub），早于水位线即拒绝——**不删键**，被踢后重新登录（IssuedUtc 更晚）自然通过。
   （`IssuedUtc` 随 Cookie 滑动续期更新，但校验发生在续期之前，所以判定成立。）
   Cookie 缺 `IssuedUtc`（正常不会出现）按"最早"处理：存在水位线即拒绝（fail-closed）。
4. **XADD `auth:kick-requests`**：消息 `KickRequestMessage(SubjectId, Targets, OperatorId, KickedAtUtc)`，
   `Targets` 按被删 `refresh_token` 行的 sid 拆分：每个浏览器会话一个 `KickTarget(sid, clientIds)`，
   无 sid 的 App 登录合为一个 `sid=null` 目标。仅在存在目标时发送。
5. **删行**：按**同一过滤条件** `ExecuteDeleteAsync`
   （不按快照主键删——否则快照后并发 refresh 新写入的行会残留一条有效 token）。
6. **SSO `KickRequestConsumer`**（异步，与上面解耦）：XREADGROUP → 逐目标调用 `IBackChannelLogoutService`（带 sid，
   满足 `BackChannelLogoutSessionRequired` 客户端）→ XACK。
   - 幂等粒度为 消息×目标，**两段式**：先 `SETNX` 一个 2 分钟的处理租约，发送成功再改写为 7 天的已完成标记，发送失败释放租约。
     进程在"抢到标记→发送完成"之间崩溃时租约自行过期，消息被 `XAUTOCLAIM` 重新认领后可重发（直接写 7 天标记会让这个窗口里的
     通知永久丢失）；租约被他人持有的目标本轮不处理、整条消息不 ACK（不能当成已完成）。租约 TTL 必须小于认领空闲阈值
     （5 分钟），`KickRequestConsumerTests` 锚定该约束；
   - 单条失败不 ACK、不影响同批其他消息；
   - 消费组的创建与 `NOGROUP` 后的重建都在循环内、由统一的错误兜底重试：BackgroundService 里逃出 `ExecuteAsync` 的异常默认会
     让宿主停机，SSO 是登录入口，不能被踢人通知这条旁路的 Redis 抖动带走；
   - `XAUTOCLAIM` 每 5 分钟认领 pending（含宕机实例遗留），超过 5 次投递转死信并 ACK；
     **Redis < 6.2 没有 XAUTOCLAIM 时自动降级为 `XPENDING` + `XCLAIM`**（5.0+；客户端侧按空闲时长筛选，
     因为 XPENDING 的 IDLE 选项同样要 6.2+），已在 Redis 5.0.14 / 6.2.18 实测；
   - 消费组首次创建从 `0-0` 起，SSO 首次启动前已写入的通知也会补发；
   - 每条消息开独立 DI scope 取 `IBackChannelLogoutService`（依赖链含 Scoped 的 `IMasaStackConfig`）。
7. **审计**：`OperationTypes.SignOut` + 描述（操作者/被踢者/粒度/行数/客户端），失败不阻断踢人。
8. 已删除：匿名 SSO 端点 `LogoutUserAsync` / `IsInternalNetwork`、`kicked:{sub}` 键。
   Admin 的 `SsoAddr` HttpClient 已随 Swagger 的 ROPC 登录（`LoginByAccountAsync`）一并移除：Swagger 改用框架自带的 Authorize 弹窗，手动粘贴 SSO 签发的 access_token。

### IS4 backchannel 的真实约束（反编译 IdentityServer4 4.1.2 确认）

- `DefaultBackChannelLogoutService.CreateTokenAsync` 用的 `IssueJwtAsync(lifetime, claims)` 依赖
  `HttpContext` 推导 issuer——**在 BackgroundService 里会抛 `ArgumentNullException`**。
  → `KickBackChannelLogoutService` 继承默认实现，无 HttpContext 时改走三参重载
  `IssueJwtAsync(lifetime, issuer, claims)`；issuer 取 `IdentityServerOptions.IssuerUri`，
  未配置则取 `IMasaStackConfig.GetSsoDomain()`（与 Admin 校验令牌的 Authority 同源）。
  有 HttpContext 的请求内场景行为不变。
- 客户端 `BackChannelLogoutSessionRequired`（默认 true）且通知没有 sid 时，IS4 抛
  `ArgumentException("Client requires SessionId")`。→ 按 sid 扇出；仍无 sid 的 App 目标上，
  要求 sid 的客户端被跳过并告警（App 会话没有浏览器 sid，不存在 sid 可发）。
- IS4 的 backchannel HTTP 客户端**吞掉对客户端的 HTTP 失败**（只记日志、不抛），所以"客户端不可达"
  **不会重试**；消费者重试只覆盖 SSO 自身/Redis 故障/令牌签发失败。
- backchannel 的目标客户端集合 = 被删 `refresh_token` 行的 ClientId ∩ 配置了 `BackChannelLogoutUri` 的客户端。
  只有 `authorization_code`/`reference_token` 而无 refresh_token 的客户端不会收到通知
  （其 token 已随行删除失效，只是少了近实时的客户端登出）。

踢人后：App 下次 refresh → `invalid_grant`；Web 客户端收 backchannel logout_token（近实时）；
SSO 门户 Cookie 下次请求被拒；JWT AT 过期前仍可调 API——**App 的收敛时间 = 剩余 AT 寿命（+ 资源侧 JwtBearer 默认 5 分钟
时钟偏差）**，平均约为一半。这个窗口的大小由客户端 `AccessTokenLifetime` 决定（IS4 与 Masa 实体默认 3600s，
即最坏约 65 分钟；缩到 900s 后最坏约 20 分钟），配置方法与边界见 §7；个别客户端可启用 `AccessTokenType=Reference`
获得删除即 401，代价是资源侧 introspection。

## 3. 端点与权限

- `GET api/online-user/GetList` / `GET api/online-user/{subjectId}/sessions` / `POST api/online-user/kick`
- **鉴权靠处理方法上的特性，不靠 `RouteHandlerBuilder`**：Masa 的 `ServiceBase.RouteHandlerBuilder` 只在
  `AutoMapRoute` 里被读取，而 `Program.cs` 开了 `DisableAutoMapRoute`，显式的 `MapGet/MapPost` 不会应用它
  （实测：endpoint 元数据里没有任何授权信息，等于匿名可访问；全局也没有 `FallbackPolicy` 兜底）。
  所以构造函数里不再写 `RouteHandlerBuilder = ...RequireAuthorization()`（它没有作用，反而造成"已受保护"的错觉）。
  - 两个查询接口：`[Authorize]`——只校验登录（未登录 401），与现有列表接口的口径一致，不需要权限码
    （曾设计过 `api.online-user.list`，已去掉）。
  - 踢人接口：`[MasaAuthorize]`（**不写 Code**）——与 `TeamService.CreateAsync`（TeamAdd）同一套机制，已实测
    方法上的 `[MasaAuthorize]` 会进 endpoint 元数据（早先"不进元数据"的判断是错的）。
    判定过程：`CodeAuthorizationMiddlewareResultHandler` 按请求路径推导权限码 `{serviceId}.api.online-user.kick`，
    与用户的 Api 权限（`ReplenishCode = {AppId}.{Code}`，经 `WildCardContains` 通配匹配）比对，未命中 403。
    **不要显式写 Code**：显式 Code 不会被加 `{appId}.` 前缀，而用户权限码带前缀，会永远匹配不上。
  - 因此种子权限 Code 必须与"路径推导结果"一致；改路由必须同步改 `ElementPermissionCodes.ApiOnlineUserKick`
    （`OnlineUserAuthorizationTests` 固定这条约束）。
- 与 TeamAdd 一致意味着：内置 admin 账号自动拥有全部权限（无需绑角色）；权限码匹配支持通配符 `*` / `?`；
  用户权限码列表有 5 秒滑动缓存（权限变更最多约 5 秒后生效）。
  （注意：`WildCardContains` 是未锚定的子串匹配，这是 `[MasaAuthorize]` 机制本身的特性，对所有接口一视同仁。）
- 种子：`AuthSeedData` 的 `#region Auth` 里与 `teamAddApi` 同写法定义 Api 权限 `api.online-user.kick`
  （`onlineUserKickApi`），加入 `authMenus` 一并种子。注意三点：
  - 与其余 Auth 权限共用同一个 `!Any(SystemId == Auth)` 守卫：**只有全新初始化的库会种上，已初始化过的存量库不会补种**。
    内置 admin 的权限来自"全部权限缓存"，权限行不在库里时 admin 调踢人接口也是 403，存量库需另行补这条权限行。
  - 它是裸 Api 权限，没有像 TeamAdd 那样挂 Element 并 `BindApiPermission`。普通角色拿 Api 权限的路径是 Element 的关联
    （见 `GroupPermissionsByUserAsync`），所以目前只有内置 admin 能放行；要放行普通角色，需补一个 Element 权限并绑定。
    （角色管理 UI 在本仓库之外，裸 Api 权限能否直接绑给角色未确认。）
  （若某环境在去掉 list 权限之前已启动过 Admin，库里会残留一条 `api.online-user.list` 权限行，
  无任何代码引用，可在角色管理里手动删除，不删也无害。）

## 4. API 契约变更（破坏性，调用方在本仓库之外）

Admin 前端已在 #1467 中移出本仓库，以下变更需通知消费方：

- `OnlineUserDto` / `UserSessionDto` 为新的分组结构（按用户 → 会话；`IsOnline`、`Channel`、`GrantKey`）；
- `UserSessionDto.LastActiveAt` → **`LastRenewedAt`**（语义修正：最近一次 refresh_token 轮换时刻，
  不是"最近操作时刻"，误差上界约等于客户端 `AccessTokenLifetime`）。`LoginTime` 保留但语义澄清为
  "本次登录时刻，同族恒定"。前端字段名需要同步改。
- 列表新增 `OnlyOnline`（默认 **true**）：默认只返回在线用户；传 `false` 可查"仍挂着会话但已闲置"的用户。
  这是行为变更——此前列表会把 30 天内登录过、早已不活跃的用户全带出来。
- 踢人请求 `KickUserModel` → `KickOnlineUserModel(SubjectId, SessionId?, ClientId?, GrantKey?)`；
  **单独的 `ClientId` 现在返回 400**；**`GrantKey + SessionId` 同时出现也返回 400**；
- 踢人响应 `KickOnlineUserResultDto(RemovedGrants, ClientIds)`：`RemovedGrants` 是**删除的 grant 行数**
  （含已消费的轮换旧行与 `authorization_code` 等非会话行），不含 `user_consent`，不等于会话数；
  为 0 不代表失败（水位线与通知仍已生效）。
- 列表 `Channel` 过滤（Web / App）作用在会话行上：返回用户的 `Sessions` / `SessionCount` / `IsOnline`
  **只按该渠道计算**，没有该渠道会话的用户不返回（此前是"筛用户但仍带全部会话"，易被误读）。
  不传 Channel 时行为不变；`GET {subjectId}/sessions` 始终返回全部渠道。

## 5. 验证状态

自动化（全绿）：

- `Masa.Auth.EntityFrameworkCore.Tests`：IS4 store 写/读/删 `sso_session` 自定义 Type、按 Subject 全删、
  按 sid 删、TokenCleanup 按 Expiration 清理不分 Type（IS4 store 自身语义；Admin 踢全部另行保留 consent）。
- `Masa.Auth.Service.Admin.Tests`（`OnlineUserSessionTests` / `OnlineUserAuthorizationTests`）：
  会话分组/渠道/阈值；**活跃时间语义**（同族 `CreationTime` 相同、仅 `ConsumedTime` 递增的真实 IS4 形态；
  长会话持续续期仍在线；整组已消费不展示；从未续期退化为登录时刻；`OnlyOnline` 开关）；
  四级踢人过滤与 SubjectId 隔离；**所有分支都保留 `user_consent`**；
  仅 ClientId 不匹配任何行；`GrantKey + SessionId` 被拒；`ExecuteDeleteAsync` 实际删除结果；`ValidateKick`；水位线 sid 集合；
  backchannel 目标按 sid 拆分；渠道过滤（会话/计数/在线判定按渠道计算）；鉴权约束（三个接口的授权特性都在、踢人的 `[MasaAuthorize]` 不带
  Code/Roles、种子权限码与路由推导结果经真实匹配器对得上）。权限放行/403 的运行时行为由框架执行，
  这里不重复测，发布前需手工验证（见下）。
- `OnlineUserKickFlowTests`：`KickUserAsync` 的步骤顺序（Redis 写入时行仍在、删行最后）；
  Redis 在水位线/Stream 任一步失败时 DB 毫无变化，重试能重建 sid 水位线与通知目标。
- `Masa.Auth.EntityFrameworkCore.Tests` 的 `CustomTokenRequestValidatorGrantCleanupTests`（9 个用例；该测试项目为此引用了
  `Masa.Auth.Web.Sso`）：用**真实的 `CustomTokenRequestValidator` + 真实 IS4 `PersistedGrantDbContext`（SQLite 内存库）**
  执行两处清理——
  - refresh 修剪：只删目标链上的已消费行、存活行保留；不动其他 Type（`user_consent` / `authorization_code` / `sso_session`）、
    其他用户 / 客户端；Web 链按 sid 精确匹配且不波及 `SessionId` 为空的 App 链；`ReUse` 客户端不修剪；
    模拟一次完整轮换后链上只剩 2 行且已消费行的 `ConsumedTime` 保留；
  - 单活 App 会话：password 登录清掉同 (sub, client) 的 App 链整条（存活 + 已消费），Web 链 / 其他客户端 / 其他用户 / 其他 Type 不动；
    `impersonation`、本次请求没申请 `offline_access`（含"客户端允许但请求没带该 scope"与 scope 校验结果缺失两种）、
    带 sid 的 `authorization_code` 几种情形不删任何行；
  - 数据库不可用时两处清理都吞掉异常，不阻断登录 / 刷新。
  这组测试用变异验证过（去掉 `ConsumedTime` 条件、把 `SessionId IS NULL` 当"不过滤"、单活谓词去掉 `SessionId IS NULL`
  各自都会让测试失败），并锚定 §1 的实体类型绑错问题。
- `KickRequestConsumerTests`（同一测试项目，用内存 Redis 桩，不连真实 Redis）：两段式租约（发送瞬间标记是短 TTL 租约、
  成功后为 7 天完成标记）、崩溃后租约过期可补发且不重复、租约被他人持有时不 ACK、发送失败释放租约、释放失败不盖掉原始异常、
  毒消息被 ACK、消费组创建 / 重建失败不让 `ExecuteAsync` 退出。变异验证过（把租约改回 7 天标记、把"推迟"当成已完成、
  把建组放回循环外 / catch 块内各自都会让测试失败）。
- `MixedBearerAuthenticationTests`：形状分流，**并对着 IS4 真实的 `DefaultHandleGenerationService` 生成 500 个句柄**确认全部被识别为句柄。
- 未覆盖：SSO 侧的 `KickBackChannelLogoutService` / `SsoKickWatermark` 依赖
  Redis 与 IS4 运行时，仓库内无对应测试，靠下面的手工冒烟。

**发布前手工冒烟（需运行环境，尚未执行）：**

1. 踢人删行后立即 refresh → 必须 `invalid_grant`（运行时最终确认 grant store 唯一）。
2. Chrome + Safari + ROPC 登录同一账号 → 列表 1 用户、Web 按 sid 分组、App 按行展开。
3. 踢单条 App grant / 整浏览器 / 全部的实际效果。整浏览器要特别确认：被踢浏览器的下一次请求被拒并要求重新登录，
   同账号的另一个浏览器不受影响（sid 水位线读的是 Cookie 票据 `Properties.Items["session_id"]`，不是 `sid` claim）。
4. **backchannel 真实送达**：准备一个配置了 `BackChannelLogoutUri` 的 Web 客户端，踢人后确认该地址
   收到 `logout_token`，且 `iss` 与 SSO 对外地址一致、含 `sid`（这是 BackgroundService 无 HttpContext 路径的首次真实验证）。
5. 踢后立即重新登录成功；多浏览器旧 Cookie 持续被拒（水位线不删键）。
6. 踢全部后用户的 consent 仍在（再次授权不弹同意页）。
7. SSO 宕机时踢人 → 行与水位线已生效，恢复后补发 backchannel；重复投递不重复通知。
8. 鉴权（框架执行，单元测试覆盖不到，必须实测）：
   - 未登录（无 token）调用三个接口均返回 **401**（重构前两个查询接口实际是匿名可访问的）；
   - 内置 admin 账号踢人放行（全新初始化的库；存量库需先补权限行，见 §3 种子说明）；无 `api.online-user.kick` 权限的账号返回 **403**；
   - 403 时 Admin 日志会打印 `authentication failed code ...`，核对它等于 `{serviceId}.api.online-user.kick`；
   - 列表 / 会话接口登录即可访问（不需要权限码）。
9. `GET/POST /LogoutUserAsync` 均返回 404。
10. **Reference 客户端踢人即时失效（introspection 链路）**：配置 `IdentityServer:IntrospectionClientId/Secret`（DCC）并
    将测试客户端设为 `AccessTokenType=Reference` 后：用该客户端的 opaque 令牌调 Admin API 正常 200；踢人（删行）后
    **同一令牌的下一次请求立即 401**（默认无缓存）；`IntrospectionCacheSeconds>0` 时 401 在缓存到期内出现。
11. **JWT 客户端不受接线改造影响**：未切 Reference 的客户端（JWT 令牌）调 Admin API 行为与改造前完全一致
    （含证书豁免、claims 映射、`[MasaAuthorize]` 403 判定）；introspection 密钥未配置时 Admin 启动无异常。
12. **claims 对齐**（introspection 响应 vs JWT 本地解析）：同一用户分别用 JWT 与 opaque 令牌调
    `GET /api/online-user/GetList`，确认 `IUserContext` 的 UserId/Account/StaffId 一致、`[MasaAuthorize]` 权限判定一致。
10. **App 踢人在 AT 寿命内收敛**（需先按 §7 把 App 客户端 AT 缩短并 `SyncOidc`）：App 登录后拿到 AT，Admin 踢该 App 会话，
    记录时间；AT 仍可调 API 直到过期；AT 过期后 refresh 必须得 `invalid_grant`。最迟收敛时间应 ≤ AT 寿命 + 5 分钟。
11. **refresh 修剪**：同一 App 会话连续 refresh 5 次以上，每次后查 `auth.PersistedGrant`（`Type='refresh_token'`、该 sub+client、
    `SessionId IS NULL`）→ 稳态只剩 **2 行**（1 存活 + 1 刚消费）；在线列表该会话仍显示在线、`LastRenewedAt` 随每次 refresh 前进；
    同账号的另一个客户端 / 浏览器会话的行数不变；踢人后 refresh 仍得 `invalid_grant`。
12. **单活 App 会话（修复后首次真实验证）**：同账号同 App 客户端连续两次 password 登录（scope 含 `offline_access`）→ 旧 refresh_token 行已被删、
    旧设备 refresh 得 `invalid_grant`、列表只剩 1 条 App 行；SSO 日志出现 `Single-active App session policy: revoked ...`
    而不是 Warning。

## 6. 已知限制 / 待决

### 活跃时间只能近似（重要）

"最近续期时刻"来自已消费旧行的 `ConsumedTime`（每次 refresh_token 轮换写一次）。这带来三类固有偏差：

- **误差上界 ≈ 客户端的 `AccessTokenLifetime`**：续期只在访问令牌过期时才发生，所以"最近续期"最多比真实
  使用晚一个 AT 寿命（默认 1 小时；阈值默认 120 分钟 = 2 倍 AT，留了余量）。客户端若把 `AccessTokenLifetime`
  调到 12 小时，就必须同步用 DCC 的 `OnlineUser:OnlineThresholdMinutes` 把阈值调大，否则正常使用中的用户会被判离线。
- **`RefreshTokenUsage = ReUse` 的客户端没有信号**：不轮换 → 不写 `ConsumedTime`，活跃时间退化为登录时刻
  （登录超过阈值即显示离线，即便一直在用）。Masa `Client` 实体与 IS4 默认都是 `OneTimeOnly`，现网成立；
  但 `ClientDetailDto` 暴露了该字段，管理员可以改。**本次只在文档标注，不做运行时检测**——运维侧应约定不把
  需要在线展示的客户端设为 `ReUse`。
- **纯门户用户**：只有 `sso_session` 行、没有 `offline_access` 客户端时，`CreationTime` 只在登录时写一次，
  不随门户活动刷新 → 登录超过阈值后 `IsOnline=false`，即使 SSO Cookie 仍有效。修复需要在门户请求热路径写库，
  风险与收益不成比例，暂不处理；该用户仍在列表中（`OnlyOnline=false` 时）且可被踢。

### 其他

- 列表接口的搜索与分页仍在内存里做；候选用户的粗筛已下推到 SQL：只取 `DISTINCT SubjectId`——默认在线列表取"有续期时刻晚于阈值的行"
  的用户（规模 ≈ 在线人数），`OnlyOnline=false` 取"有存活行"的用户——不再对整张 `PersistedGrant` 做 `GROUP BY` 聚合、
  回传每个有 grant 的用户一行（闲置 grant 默认保留 30 天，远多于在线会话，旧做法的开销随闲置量增长）。载入候选用户的行之后
  再在内存里精确复核（存活会话、按存活会话的最近续期判定在线），画像只取最终出现在结果里的用户。
  `PersistedGrant` 上没有 `CreationTime` / `ConsumedTime` 的索引，粗筛仍是一次带过滤条件的扫描（只是不再聚合、回传很少的行）；
  在线人数达到万级、或表规模很大时，可再评估加索引（需要 PG / SqlServer 各一份迁移）并把 `Search` 与画像查询一并下推。
- backchannel 仅覆盖有 refresh_token 且配置了 `BackChannelLogoutUri` 的客户端；客户端侧 HTTP 失败不重试。
- **App 会话实际上收不到 backchannel**：`BackChannelLogoutSessionRequired` 在 IS4 与 Masa 实体上都默认 `true`，
  而无 sid 的 App 目标无法满足该要求，`KickBackChannelLogoutService` 会跳过并告警。
  所以"近实时下线"只对 Web（code 流）成立，App 只能在下一次 refresh 时拿 `invalid_grant`。
  **对 App 的承诺是"在 AT 寿命内收敛"，不承诺实时**：被踢后 App 仍可凭未过期的 AT 调用 API，直到 AT 过期并 refresh 失败
  （最坏 ≈ AT 寿命 + 5 分钟时钟偏差，平均约一半）。前端 / 接口文档 / 运营口径都不要写"立即下线"；
  要把窗口压到分钟级，靠缩短 App 客户端 AT（§7），而不是靠通知。

#### 已评估、暂不做的 App 实时性升级路径

当前目标是**分钟级收敛**且**没有可复用的长连接 / 推送通道**，所以只做"服务端权威状态 + 短 AT"这一层。以下路径评估过，
需求升级到"秒级"时再启动，不要在没有明确需求时先做：

1. **资源 API 层 `(sub, clientId)` 水位线**：踢人时写 Redis 水位线（与现有 SSO Cookie 水位线同模式，键换成 `(sub, clientId)`，
   TTL ≥ AT 寿命即可），各资源 API 校验 JWT 时比对 `iat`/`auth_time` 早于水位线即 401。能把窗口降到接近 0，
   代价：每个资源 API 每请求多一次 Redis 读，且**所有**接受该 App 令牌的资源服务都要接入同一个校验中间件，漏接入一个就是旁路
   （Admin 的 JwtBearer 关了 `ValidateAudience`，任何 SSO 令牌都能进 Admin；其他资源服务的受众校验情况需逐个确认）。
   当前 JWT 本地验签的无状态优势会丢一部分。
2. **推送 / 长连接通知**（FCM / APNs 静默推送、WebSocket、SSE）：Admin 踢人后推一条"会话失效"让 App 立刻 refresh 并发现被踢。
   代价：需要新建并运维通道、设备 token 管理，且推送是尽力而为（不可靠），仍须配合路径 1 或短 AT 兜底。本仓库目前没有这类通道。
3. **`AccessTokenType=Reference`**（逐客户端）：删除 grant 即 401，但每次 API 调用都要走 introspection，成本与路径 1 同量级。
4. 行业参考（仅供对照）：跨服务的会话撤销事件有 OpenID **Shared Signals / CAEP**，Microsoft 的 Continuous Access Evaluation
   走的是"资源端订阅撤销事件 + 短 AT"，本质与路径 1 + 2 的组合同类。

**阈值**：`OnlineUser:OnlineThresholdMinutes`（DCC）沿用默认 120，不因缩短 App AT 而调整。该阈值全局生效、Web 与 App 共用：
若调到小于 2 倍 Web 客户端 AT（默认 3600s → 120 分钟），Web 用户会在两次续期之间被显示成离线。保持 120 的代价只是
App 用户空闲后最多多显示约 2 小时"在线"，纯展示问题，不影响踢人。
- 踢人的"快照"与"删行"是两次查询：二者之间新登录产生的行会被删掉（删行按过滤条件而非快照主键，令牌一定失效），
  但不在快照里，所以不会进 backchannel 目标与审计的客户端清单。要消除需要事务/`DELETE ... RETURNING`
  （PG 与 SqlServer 写法不同，EF 不支持），收益仅是极窄竞态下的通知完整性，暂不处理。
- `KickRequestConsumer` 对"消息已被 MAXLEN 裁剪、但仍在 PEL 里"的条目只跳过不 ACK（XCLAIM 对这类条目返回
  不带 Id 的空条目，无法直接 ACK，Redis 5.0 与 6.2 实测都不会自动清理）。触发条件是消费者宕机期间 Stream 积压超过约 1 万条，
  踢人是低频操作，暂不处理。
- sid 水位线只按"实际被删行"的 sid 写，不按请求里的 `SessionId` 直接写：sid 水位线键里没有 SubjectId，
  直接写会让"主体 B + 主体 A 的 sid"这种请求误踢 A 的浏览器。因此整浏览器踢人若没匹配到任何行
  （例如该浏览器在上线前登录、没有 `sso_session` 行），不会拒绝它的门户 Cookie；需要强制拒绝时请用"踢全部"（写 sub 水位线）。

## 7. App 实时性分层 SOP（踢人生效时延的运营指南）

踢人链路保证 **refresh_token 删除即失效**，但对 App 的强制力上界是手里的 access_token：
资源端本地校验 JWT 时不问 SSO，"有效性"刻在令牌自身（签名 + exp）。行业（Auth0 / Okta / Cognito / Keycloak /
Firebase）全部是同一语义——**App 端踢人窗口 = AT 剩余寿命**。压缩窗口的手段按层叠加，不要跳级上重方案：

- **L0 撤 refresh_token（已实现，一切方案的底座）**：App 下一次 refresh 得 `invalid_grant` → 本地登出。
  Web 渠道已近实时（backchannel 秒级送达 + 门户 Cookie 水位线立即拒）；App 渠道窗口 = AT 剩余寿命（默认 ≤ 1h）。
- **L1 缩短 App 客户端 AT 寿命（零代码，推荐首选）**：管理端把 App 客户端 `AccessTokenLifetime` 从默认 3600s
  调到 600-900s（10-15 分钟），窗口随之降到分钟级。代价：`/connect/token` 的 refresh QPS 约升到原来的
  4-6 倍（1h → 15min），按在线 App 用户数估算容量；另需确认在线阈值联动（见下节）。
- **L2 推送通知触发本地登出（UX 改进，不是强制手段）**：踢人后经 FCM/APNs/厂商通道推送"你已被下线"，
  App 收到即本地登出。推送不保证送达（杀进程/拒授权），强制力仍由 L0/L1 兜底——产品文案不要承诺"立即下线"。
  推送基础设施不在本仓库，仅在此登记为接入点（消息可复用 `KickRequestConsumer` 的处理位置扇出）。
- **L3 reference token + introspection（个别高安全客户端，见 §8）**：该客户端的 AT 改为 opaque 句柄，
  资源端每请求（或每缓存周期）问 SSO `/connect/introspect`，踢人删行后下一次请求即 401。
  **启用前提：该客户端的整个 API 面都已接 introspection（或走统一网关）**，否则未接入的服务会拒收 opaque 令牌。
- **L4 长连接踢下线通道（IM 级实时，暂不建设）**：SignalR/WebSocket/MQTT 推踢下线事件，秒级强制；
  钉钉/微信类产品能力，需要连接基础设施与 App 常驻连接，超出 IdP 范畴。

### 在线阈值与 AT 寿命的联动公式

DCC `OnlineUser:OnlineThresholdMinutes` **必须 ≥ 2 × 租户内启用客户端的最大 AccessTokenLifetime（分钟）**：
续期（refresh）只在 AT 过期时发生，"两次续期间隔"的上界 = AT 寿命；阈值小于 2 × AT 时，
正常使用中的用户会在下一次续期之前被误判离线（`OnlyOnline` 列表看不到、`IsOnline=false`）。

- 默认 120 分钟 = 2 × 3600s，公式成立；
- L1 把 App 客户端调到 15 分钟后**不需要**调阈值（120 ≥ 30，公式仍满足，只是余量变大）；
- 任何客户端把 AT 调大到 1 小时以上（如 2h），阈值必须同步调到 ≥ 240 分钟；
- Admin 列表/会话接口已内置运行时告警：阈值低于 2 × 最大启用客户端 AT 时每小时打一条
  `Online threshold ... is below 2 x ...` Warning（`OnlineUserService.WarnThresholdTooSmall`），
  专门捕捉"改了客户端忘了改阈值"的漂移。告警不改变裁剪行为，判定是纯函数
  （`SuggestedOnlineThresholdMinutes` / `ThresholdTooSmall`，测试锚定）。

### App 端处理契约（客户端必须实现）

1. 携 AT 调 API 得 **401** → 先做一次 refresh；refresh 得 `invalid_grant`（或 400）→ **本地登出**，禁止无限重试；
2. `invalid_grant` 本身就等价于"会话被终结"——踢人、单活策略顶号、撤销三者在 App 侧同形，统一按登出处理；
3. 被顶号（单活策略）与被踢人对 App 是同一体验：静默登出到登录页，不弹错误对话框。

## 8. Reference Token + Introspection SOP（L3 落地，踢人即时 401）

原理：`AccessTokenType=Reference` 的客户端，其 AT 不是 JWT 而是**指向 auth.PersistedGrant `reference_token` 行的
opaque 句柄**——放弃了"自证明"能力来换"中心吊销"能力。资源端无法本地校验它，每请求（或每缓存周期）拿它问
SSO `/connect/introspect`；踢人删除该行（`FilterForKick` 现有行为，已测试锚定）后，**下一次 introspection 即返回
inactive → 401**，踢人窗口 = 0（或缓存秒数）。签发侧 SSO 零开发（IS4 内置端点 + 客户端配置项），改造全部在**校验侧**。

### 上线约束（先读这一条再决定给哪个客户端开）

**客户端切 Reference 是"全 API 面"承诺**：opaque 令牌只能被已接 introspection 的资源端校验，该客户端调用的所有
服务（含其他仓库的 Masa 服务/网关）未接入就直接全量 401。启用顺序：
1. 先让目标客户端的**全部**被调服务拷贝 Admin 的接线（见下），各自在 DCC 配好密钥；
2. 确认这些服务运行正常（此时所有 JWT 客户端不受任何影响——接线对 JWT 透明）；
3. 最后才在客户端管理（`ClientDetailDto.AccessTokenType`）把目标客户端切成 `Reference`。
   有统一网关时优先只改网关。回滚 = 把客户端切回 `Jwt`，立即生效。

### 接入步骤

- **SSO/签发侧（零代码）**：管理端给 auth API 对应的 **ApiResource** 配置 SharedSecret（`api/sso/apiResource`，
  `ApiResourceDetailDto.Secrets`；IS4 约定 introspection 调用方以 **ApiResource 名为 ClientId、其 SharedSecret 为
  ClientSecret** 认证）。
- **资源侧（Admin 已落地，其他服务拷贝三处）**：
  1. 引包 `Duende.AspNetCore.Authentication.OAuth2Introspection`（原 IdentityModel 包已归档下架，Duende 是
     IdentityServer 团队官方继任者，7.0.2 带 net10.0 TFM；经 `Directory.Packages.props` 统一版本）；
  2. 拷贝 `src/Services/Masa.Auth.Service.Admin/Infrastructure/Auth/MixedBearerAuthentication.cs`（自包含，含
     `AuthSchemes`、`TokenSchemeSelector`、`AddMixedBearerAuthentication`）；
  3. `Program.cs` 把 `AddJwtBearer("Bearer", ...)` 换成 `AddMixedBearerAuthentication(ssoUrl, dccDefaultConfig, jwtOptions => { 原JWT配置原样搬入 })`。
     未配密钥时行为与历史完全一致，可以先合代码后配密钥。
- **DCC 配置**：`IdentityServer:IntrospectionClientId`（ApiResource 名）/ `IdentityServer:IntrospectionClientSecret`
  （其 SharedSecret）配齐即启用；`IdentityServer:IntrospectionCacheSeconds` 默认 0=关缓存。

### 行为细节（实现锚定，排障先看这里）

- **缓存默认关闭**：Duende 包默认缓存 active/inactive 结果 **5 分钟**，会把踢人生效拖到 ≤5 分钟，因此默认以
  `HybridCacheEntryFlags.DisableLocalCacheWrite | DisableDistributedCacheWrite` 禁写两层（永不命中）；
  `IntrospectionCacheSeconds>0` 时开启按秒缓存，踢人窗口变为 ≤ 该秒数（RFC 7662 要求缓存不超 exp，包已处理）。
- **分流**：`TokenSchemeSelector` 按令牌形状路由——恰好三段非空 '.' 分段 → jwt 本地校验；形如 reference 句柄
  （长度 16～128、全部为 base64url 字符 `A-Za-z0-9-_`；IS4 默认句柄是 43 字符）→ introspection；**两者都不像的垃圾凭据
  也走 jwt，在本地 401，不向 SSO 发 introspection 请求**。introspection 默认不缓存（连 inactive 也不缓存），若任何非 JWT 形状都转发，
  匿名调用方随便填个字符串就能让资源端对 SSO 发一次 HTTP 往返，等于把 SSO 暴露成可被放大的目标。
  仍然剩下的口子：形状合法的随机句柄照常被转发，要进一步收敛请开 `IntrospectionCacheSeconds` 或在网关限流。
  若自定义了 `IHandleGenerationService` 且输出超出上述区间 / 字符集，Reference 客户端会被本地 401——改生成器时同步改
  `TokenSchemeSelector.LooksLikeReferenceHandle`。无 Bearer 凭据走 jwt（历史行为）。测试锚定于 `MixedBearerAuthenticationTests`。
- **对外 scheme 名不变**：默认 scheme 仍是 "Bearer"（PolicyScheme），`[Authorize]`、挑战头、现有调用方无感。
- **claims 对齐**：introspection 响应按 claim 原名透传（reference token 存的就是签发时的全量 claims），与 JWT +
  `MapInboundClaims=false` 同口径；`NameClaimType=IdentityClaimConsts.USER_NAME` 对齐 Masa identity。冒烟 §5.12 核验。
- **HTTP 细节**：introspection 后端走包内命名客户端（10s 超时 + 自签证书豁免，镜像 JwtBearer 的 BackchannelHttpHandler；
  .NET 10 移除了 `ConfigurePrimaryHttpHandler`，主处理器经 `UseSocketsHttpHandler(Action<SocketsHttpHandler, IServiceProvider>)` 配置）；
  discovery 经 `DiscoveryPolicy.RequireHttps=false` 发现 `/connect/introspect`。
- **容量**：缓存关闭时每个 Reference 客户端请求多一次 SSO introspection 往返（进程内对同 token 并发请求有
  in-flight 去重）；introspection 端点 QPS ≈ 该客户端 API QPS。SSO 不可用时这些请求 401——Reference 客户端的
  可用性与 SSO 强耦合，这是"即时吊销"换来的代价，高安全客户端才值得。
- **与 §7 的组合**：Reference 客户端的实时性由 introspection 保证，AT 寿命可维持默认（不必调短）；短 AT（L1）
  只用于仍走 JWT 的 App 客户端。两者不冲突。

## 7. App 接入约定与客户端配置

对 App 的承诺见 §6：**AT 寿命内收敛，不承诺实时**。本节是把这个窗口压到分钟级所需的配置，以及 App 端必须遵守的约定。

### 7.1 缩短 App 客户端的 AccessTokenLifetime（配置，非代码）

**现状（已核对代码）**：

- Masa `Client` 实体默认 `AccessTokenLifetime=3600`、`RefreshTokenUsage=OneTimeOnly`、`RefreshTokenExpiration=Sliding`；
  属性是 private setter，由 DTO 经 Mapster 写入。
- `AddClientDto` **没有** `AccessTokenLifetime` 字段，新建客户端一律是 3600；`ClientDetailDto.AccessTokenLifetime`
  在客户端编辑时可改。
- 不在代码里给 `Native` 客户端默认 900，原因：实体来自外部包；且 `ClientType`（Web / Spa / Native / Machine / Device）描述的是
  接入形态，与"是否走 token-only 登录（App 渠道）"并不一一对应——按类型改默认值会误伤。**逐客户端配置**。

**目标值**：App 类客户端 **600～900 秒**（10～15 分钟），最坏收敛 ≈ AT + 5 分钟 ≈ 15～20 分钟；Web 客户端不动。

- 不要低于 300 秒：资源侧 JwtBearer 默认 5 分钟时钟偏差，更短的 AT 实际有效期仍 ≥ AT + 5 分钟，收益被偏差吞掉，只剩 refresh 开销。
- `RefreshTokenUsage` 必须保持 `OneTimeOnly`：在线活跃信号依赖已消费行的 `ConsumedTime`（§6），`ReUse` 会让列表判定失效。
- 开销：AT 900 秒时每个活跃 App 会话约 96 次 refresh / 天，每次 1 次插入 + 1 次标记 + 1 次修剪删除（§1）；用户规模大时先评估库压力。
- 缩短只影响**之后签发**的 AT，已签发的 AT 仍按原寿命有效，所以存量用户最多需要一个旧 AT 寿命的过渡期。

**存量 App 客户端的调整**（二选一）：

1. 在 Admin 客户端管理界面（Admin 前端在本仓库之外）编辑该客户端的 `AccessTokenLifetime`；
2. 一次性 SQL（按 `ClientId` 列出 App 客户端，不要按 `ClientType` 批量改）：

```sql
-- PostgreSQL
UPDATE auth."Client" SET "AccessTokenLifetime" = 900 WHERE "ClientId" IN ('<app-client-id>', ...);
-- SQL Server
UPDATE [auth].[Client] SET AccessTokenLifetime = 900 WHERE ClientId IN (N'<app-client-id>', ...);
```

   直改库之后必须刷新 OIDC 缓存：`POST api/sso/client/SyncOidc`（`SyncOidcCommand` → `ResetAsync`）；
   不调的话要等每天 00:00 的 `SyncOidcRedisJob`，在此之前 SSO 仍按旧值签发。

### 7.2 App 接入约定（写进 App 开发文档）

1. **HTTP 全局拦截器**：API 返回 401 且 `WWW-Authenticate` 带 `invalid_token`（AT 过期）→ 用 refresh_token 刷新一次并重放原请求；
   刷新失败则清空本地票据、回登录页。业务代码不要各自处理 401。
2. **refresh 遇到 `invalid_grant` 一律视为"被踢或会话失效"**，统一文案"登录已失效，请重新登录"。IS4 的 `invalid_grant` 不带原因，
   客户端无法区分"被管理员踢 / 被同账号新登录顶掉（§1 单活策略）/ refresh_token 过期 / 用户改密"，不要尝试区分，也不要自动重试。
3. **前台恢复时主动 refresh**：App 从后台回到前台时检查 AT 剩余寿命，不足就 refresh 一次。在后台超过 AT 寿命后，
   用户一回来就能发现被踢，而不是等第一个业务请求 401。
4. **不得缓存已消费的 refresh_token**：`OneTimeOnly` 下每次 refresh 都返回新的 refresh_token、旧的立即作废；
   必须用新值**原子替换**本地持久化的票据，只保存最新的一个。
5. **refresh 单飞**：同一时刻只允许一个 refresh 请求在途，其余请求等它的结果。`OneTimeOnly` 下并发用同一个 refresh_token
   刷新，后到的会得到 `invalid_grant`，按第 2 条处理就会把正常用户误登出；同理，refresh 响应丢失后用旧 token 重试也会得到
   `invalid_grant`（服务端已轮换，与是否清理已消费行无关），应按第 2 条处理而不是反复重试。
6. **单活语义**：同账号在同一 App 客户端上多设备登录，后登录者顶掉先登录者（§1）；被顶的设备下次 refresh 走第 2 条。
