# 模拟登录（token-exchange）接入指南

**适用场景**：管理员要以他人身份操作。发起方是一个机密客户端，凭管理员**自己的** `access_token` 换取被模拟用户的 token。

> 你的后端已经确认了某个域用户、要替他换 token，见 [sso-domain-assertion.md](./sso-domain-assertion.md)。
> 终端用户在登录页输入 AD 账号和密码，走 `grant_type=ldap`，不用看本文。

---

## 1. 准备条件与接入流程

**准备条件**

- [ ] 发起模拟的管理员账号是**启用**状态，并已被授予 `UserImpersonate` 权限（见 2.2）。
- [ ] 管理员的 `access_token` 是**正常登录**拿到的。用域身份断言换来的 token，或模拟得到的 token，都不能当 `actor_token`。
- [ ] 被模拟的用户存在，且是启用状态。

**接入流程**

| 步骤 | 谁做 | 做什么 |
| --- | --- | --- |
| 1 | SSO 管理员 | 建客户端，登记 `SharedSecret`；把 `client_id` 和 `client_secret` 交给接入方（2.1） |
| 2 | SSO 管理员 | 给允许模拟的管理员授予 `UserImpersonate` 权限（2.2） |
| 3 | 接入方 | 管理员正常登录拿到自己的 `access_token`，再用它请求 `/connect/token`（第 3 节） |

---

## 2. 配置

### 2.1 SSO 管理员：配置客户端

| 配置项 | 填什么 |
| --- | --- |
| 客户端类型 | **`Machine`**（新建时选择。其它类型会被拒绝） |
| 授权方式 | 勾选 `TokenExchange (RFC 8693, impersonation)`。`Machine` 默认只有 `client_credentials`，需自己再勾 |
| 客户端密钥 | `SharedSecret`：设一个密钥，明文交给接入方作为 `client_secret` |
| Scope | 只给必要的 |
| 允许离线请求（`AllowOfflineAccess`） | **关闭** |

### 2.2 SSO 管理员：给管理员授权

在角色或用户的授权树里，勾选 **User 菜单下的 `UserImpersonate` 元素**。勾选后会联动授予 `api.user.impersonate`，授权树上看不到这个 API 行，属于正常。

- **这个权限等同于"可以成为任何人"**，包括内置 `admin`。SSO 不会比较双方权限高低。只授予完全可信的人员，并定期核对持有者。
- 全新初始化的库会自带这个权限项。存量库不会自动补种。

---

## 3. 怎么用（接入方）

### 3.1 请求 `/connect/token`

`POST`，`Content-Type: application/x-www-form-urlencoded`：

| 参数 | 值 |
| --- | --- |
| `grant_type` | `urn:ietf:params:oauth:grant-type:token-exchange` |
| `client_id` / `client_secret` | 2.1 配的客户端 |
| `actor_token` | 管理员**自己的** `access_token`（本 SSO 签发、未过期） |
| `actor_token_type` | `urn:ietf:params:oauth:token-type:access_token` |
| `subject_token` | **被模拟用户的 Id**（Guid），不是 token |
| `subject_token_type` | `urn:masa:params:oauth:token-type:user-id` |
| `scope` | 如 `openid profile`。**必传**，且不能含 `offline_access` |
| `requested_token_type` | 可选。如果传，必须是 `urn:ietf:params:oauth:token-type:access_token` |

```bash
curl -X POST https://sso.example.com/connect/token \
  -d grant_type=urn:ietf:params:oauth:grant-type:token-exchange \
  -d client_id=masa-admin-impersonator \
  -d client_secret=<共享密钥> \
  -d "actor_token=<管理员自己的 access_token>" \
  -d "actor_token_type=urn:ietf:params:oauth:token-type:access_token" \
  -d "subject_token=<被模拟用户的 Guid>" \
  -d "subject_token_type=urn:masa:params:oauth:token-type:user-id" \
  -d "scope=openid profile"
```

### 3.2 结果与使用要点

**成功时**返回 `access_token`、`expires_in` 和 `issued_token_type`，**没有** `refresh_token`。

- token 有效期最长 **900 秒**。客户端配置得更短则取更短的。过期后重新换。
- token 的 `sub` 是被模拟者，`act.sub` 是操作的管理员。

**使用要点**：

- 不能模拟自己。
- 请求里不能带 `offline_access`。省略 `scope` 而客户端开着"允许离线请求"时，也会被拒绝，所以一定要显式传 `scope`。

---

## 4. 管理后台的模拟登录（内置代理）

管理后台 UI 是 WASM 公开客户端，自己不能持有 `client_secret`，token-exchange 由 MASA.Auth 的 Service.Admin 代理执行：

`POST api/user/impersonate-token`，入参 `{ "userId": "<目标用户 Guid>" }`，需以管理员身份登录调用。

| 步骤 | 谁做 | 做什么 |
| --- | --- | --- |
| 1 | 部署 | 在 DCC 的 `auth-service` 应用下新建 JSON 配置对象 `Impersonation`（`{"ClientId": "...", "ClientSecret": "...", "Scope": "openid profile"}`），即 2.1 建的 Machine 客户端凭据。`Scope` 可选（空格分隔），留空时用 `openid profile`；它必须是该客户端被授予的 scope 的子集、不能含 `offline_access`，**必须包含 `openid`**（接收端要用它调 SSO 的 `/connect/userinfo` 验证 token，缺了会被拒绝），并应覆盖目标应用向后端请求 token 时所需的 scope（模拟 token 无法再换取更多 scope）。注意：**appsettings.json 和环境变量都不生效**（`AddMasaStackConfigAsync` 后 appsettings 内容被迁至 `Local` 节点，服务只从 DCC 读取该配置）。不配置则接口返回"模拟登录未配置"（文案会指明缺少的 DCC 配置对象） |
| 2 | SSO 管理员 | 给管理员授予 `UserImpersonate`（同 2.2） |
| 3 | 管理后台 UI | 调代理端点：服务端复核调用者权限后，以其 `access_token` 作为 `actor_token` 执行第 3 节的 token-exchange，返回 `{ "accessToken": "...", "expiresIn": 900 }` |

返回的 access_token 语义与 3.2 节一致（`sub`=被模拟用户、`act`=操作者、≤900s、无 refresh_token）。UI 拿到后以
`目标应用Url#access_token=...&expires_in=...`（URL fragment，不会发给服务器）交给目标应用；目标应用需支持从
fragment 引入该 token 建立模拟会话（接收端接入为各应用后续工作；未接入的应用会以操作者本人身份打开）。

**接收端（`Masa.Stack.Components`）对收到的 token 做两层校验，任一不过就丢弃、不进入模拟态：**

1. **本地预检**：`iss` 等于应用的 OIDC `Authority`（尾部 `/`、大小写忽略）；`sub`、`act.sub` 都是 Guid；带 `act`、`exp`、`openid` scope；`nbf` 不在未来；未过期。若 SSO 实际签发的 `iss` 与应用访问 SSO 的 `Authority` 不一致（例如应用走内网地址、SSO 对外域名作 `iss`），在应用的 OIDC 配置里设置 `Issuer`（`MasaOpenIdConnectOptions.Issuer`）。
2. **SSO 验证**：浏览器带着 token 请求 `{Authority}/connect/userinfo`，必须返回 200 且 `sub` 与 token 一致。签名、有效期只有 SSO 能验。无法验证（网络/CORS 失败、超时）同样不放行，操作者重新点一次「模拟登录」即可。要求应用所在的来源被 SSO 允许跨域访问 userinfo（与应用本身的 OIDC 登录是同一个条件）。
