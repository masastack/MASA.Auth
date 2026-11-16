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
