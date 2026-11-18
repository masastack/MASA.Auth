# 域身份断言（jwt-bearer）接入指南

**适用场景**：你的后端已经用 Windows 集成认证确认了某个域用户，要替他向 SSO 换 token（后端手里没有用户的 AD 密码）。

> 终端用户在登录页输入 AD 账号和密码，走 `grant_type=ldap`，不用看本文。
> 管理员要以他人身份操作，见 [sso-impersonation.md](./sso-impersonation.md)。

---

## 1. 准备条件与接入流程

**准备条件**

- [ ] 「第三方身份提供商」里已配置名为 **LDAP** 的身份源，且 root 账号能连上 AD。
- [ ] 要换 token 的域账号，**已经在 MASA 里有对应用户**。下面任一方式即可：
  - 保存 LDAP 身份源配置时会全量同步 AD 用户（另有每日定时同步）。手机号和邮箱**都为空**的 AD 账号会被跳过。
  - 该账号曾用 AD 密码走 `grant_type=ldap` 登录过一次。
- [ ] SSO 所在环境能连上 Auth 服务、Redis 和 AD。任一不通，请求都会失败（见第 4 节）。
- 断言通道**不会**替账号开户。账号还没入库会被拒绝。

**接入流程**

| 步骤 | 谁做 | 做什么 |
| --- | --- | --- |
| 1 | 接入方 | 生成密钥对，把**公钥 JWK** 交给 SSO 管理员（2.1） |
| 2 | SSO 管理员 | 建客户端，登记 `SharedSecret` 和 JWK；把 `client_id`、`client_secret` 和 **SSO 对外域名**（断言的 `aud` 要用，见 3.1）交给接入方（2.2） |
| 3 | 接入方 | 每次需要 token 时，用私钥签断言，请求 `/connect/token`（第 3 节） |

---

## 2. 配置

### 2.1 接入方：生成密钥对

**私钥自己保管**（密钥库 / KMS / 证书库），**公钥 JWK 交给 SSO 管理员**登记。

```csharp
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

using var rsa = RSA.Create(2048);
var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
    new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = "myapp-2026" });   // kid 必填，建议带年份或序号
var publicJwkJson = System.Text.Json.JsonSerializer.Serialize(publicJwk);          // 把这段 JSON 交给管理员
// 私钥导出：rsa.ExportPkcs8PrivateKeyPem()（.NET 7+），存入密钥库，不要提交到代码仓库
```

导出的 JSON 里会带 `key_ops`、`oth`、`x5c` 三个空数组，属正常，可直接登记。

**不想写代码**，也可以用现成工具生成。交给管理员的都只能是**公钥**，且必须带 `kid`：

- **本地命令行 [`step`](https://smallstep.com/docs/step-cli/reference/crypto/jwk/create/)**（推荐，私钥不出本机）：

  ```bash
  step crypto jwk create pub.json priv.json --kty RSA --size 2048 --use sig --alg RS256 --kid myapp-2026 --no-password --insecure
  ```

  `pub.json` 的内容交给管理员，`priv.json` 是私钥，自己保管。`--no-password --insecure` 会让私钥明文落盘，仅联调使用；正式环境去掉这两个参数，私钥会被加密保存，使用前需先解密。
- **网页 [mkjwk.org](https://mkjwk.org)**（**仅联调**）：打开 RSA 页签，Key Size 填 `2048`，Key Use 选 Signature，Algorithm 选 `RS256`，Key ID 选 Specify 并填上 `kid`（不能留空），点 Generate。**只复制右下角「Public Key」那一栏**，不要复制「Public and Private Keypair」，它带私钥，后台会拒绝保存。该服务在远端生成密钥，私钥会经过网络，正式环境不要用。

### 2.2 SSO 管理员：配置客户端

| 配置项 | 填什么 |
| --- | --- |
| 客户端类型 | **`Machine`**（新建时选择，只有该类型的界面提供下面这个授权方式） |
| 授权方式 | 手动勾选 `jwt-bearer`（即 `urn:ietf:params:oauth:grant-type:jwt-bearer`，界面显示名以实际为准） |
| 需要客户端密钥 | 保持**开启**。关闭后属于公开客户端，断言会被拒绝 |
| 客户端密钥（共 2 条） | ① `SharedSecret`：设一个密钥，明文交给接入方作为 `client_secret`。**保存后后台只显示哈希**，看不到明文也不能编辑，要换就删掉重建<br>② `JWK`：类型选 `JWK`，粘贴接入方给的公钥 JSON。「到期时间」留空或填未来时间，已过期的密钥不参与验签 |
| Scope | 只给必要的 |
| 允许离线请求（`AllowOfflineAccess`） | **关闭** |
| 客户端属性 `domain_assertion.account_pattern` | **强烈建议配**，限定这个应用能断言哪些账号。`\|` 分隔多个模式，支持 `*` 和 `?`，不区分大小写，整串匹配，匹配的是去掉 `域\` 前缀和 `@域名` 后缀之后的账号。例：`svc_*\|zhang?`。不配则可断言 AD 里任意账号 |

JWK 保存时会校验，不通过会提示原因（不回显内容）：

- 必须是**单个**公钥，不接受 JWK Set。
- `kty` 为 `RSA` 或 `EC`，**必须带 `kid`**。`use` 如果有，必须是 `sig`。
- 不能含私钥参数（`d`、`p`、`q` 等）。
- RSA 至少 2048 位，EC 曲线只接受 P-256 / P-384 / P-521。

也可以登记 `X509CertificateBase64`：公钥证书的 DER base64（不带 PEM 头，不接受 PFX）。`X509Thumbprint` 和 `X509Name` 不参与断言验签，不要选。

换密钥时，先新增一把 `kid` 不同的 JWK，等接入方切换后再删旧的。

---

## 3. 怎么用（接入方）

### 3.1 签一个断言

断言是一个 JWT，用你的私钥签。用 NuGet 包 `Microsoft.IdentityModel.JsonWebTokens`，在需要换 token 的地方直接写：

```csharp
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

// privateKey: RSA 私钥；keyId: 与登记的 JWK 的 kid 一致；clientId: 管理员建的客户端 id
// ssoDomain: SSO 对外域名，如 https://sso.example.com；domainAccount: 你已验明的域账号，如 mayue 或 LONSID\mayue
var now = DateTime.UtcNow;
var assertion = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
{
    Issuer = clientId,
    Audience = $"{ssoDomain.TrimEnd('/')}/connect/token",
    Subject = new ClaimsIdentity(new[] { new Claim("sub", domainAccount) }),
    IssuedAt = now,
    NotBefore = now,
    Expires = now.AddSeconds(60),
    Claims = new Dictionary<string, object> { ["jti"] = Guid.NewGuid().ToString("N") },   // 每次换 token 都要新值
    SigningCredentials = new SigningCredentials(
        new RsaSecurityKey(privateKey) { KeyId = keyId },
        SecurityAlgorithms.RsaSha256)
});
```

断言里的内容：

| 位置 | 字段 | 填什么 |
| --- | --- | --- |
| header | `alg` | `RS256` / `RS384` / `RS512`、`PS256/384/512`、`ES256/384/512` 之一。不接受 `none` 和 `HS*` |
| header | `kid` | 与管理员登记的 JWK 的 `kid` 一致 |
| payload | `iss` | 等于 `client_id` |
| payload | `sub` | 域账号。`mayue`、`LONSID\mayue`、`mayue@lonsid.cn` 都可以（会先去掉 `域\` 和 `@域名`）。不能含 `*`、`(`、`)`、`\|`、`=` 等特殊字符 |
| payload | `aud` | **`<SSO 对外域名>/connect/token`**，不带 `/connect/token` 的裸域名也接受 |
| payload | `iat`、`exp` | 必填。`exp - iat` 不超过 300 秒，建议 60 秒（手工联调可放宽到 300 秒）。服务器时间要与 SSO 同步，容忍约 60 秒偏差 |
| payload | `jti` | 必填，**每次都生成新值**，只能用一次 |
| payload | `nbf` | 可选 |

`aud` 里的"SSO 对外域名"以 **SSO 管理员提供的为准**（即 Stack 配置里 SSO 的域名），不是你请求时用的内网地址或 `localhost`。本地调试时请求发到 `localhost`，`aud` 也仍然写这个域名。拿不准时让管理员看 SSO 日志：断言被拒绝时会打印 `expectedAudiences`，其中就是 SSO 接受的 `aud`。

### 3.2 请求 `/connect/token`

`POST`，`Content-Type: application/x-www-form-urlencoded`：

| 参数 | 值 |
| --- | --- |
| `grant_type` | `urn:ietf:params:oauth:grant-type:jwt-bearer` |
| `client_id` | 管理员建的客户端 id |
| `client_secret` | `SharedSecret` 的明文（也可改用 HTTP Basic 头） |
| `assertion` | 3.1 签出的 JWT |
| `scope` | 如 `openid profile`，必须在客户端允许的 scope 内。**请显式传** |

```bash
curl -X POST https://sso.example.com/connect/token \
  -d grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer \
  -d client_id=myapp \
  -d client_secret=<共享密钥> \
  -d "scope=openid profile" \
  --data-urlencode "assertion=<JWT>"
```

在 C# 里用 `FormUrlEncodedContent` 传这几个字段即可，它会自动做 URL 编码。

**成功时**返回 `access_token`，另有 `expires_in`、`token_type`（固定 `Bearer`）和 `scope`。token 里 `sub` 是 MASA 用户 Id（不是域账号），域账号在声明 `https://masastack.com/security/identity/claims/domainName` 里，`amr` 为 `["wia"]`，不带 `refresh_token`。这是该用户的完整身份令牌（含角色），请妥善保管。调用 API 时放在请求头 `Authorization: Bearer <access_token>`。token 过期后用**新断言**重新换。

**使用要点**：

- 每次请求都重新签一个断言，不要复用。**请求失败后重试也要重签**：断言验签通过后 `jti` 就已被消耗，即使后面因账号、AD 等原因失败，同一份断言再发会提示"已被使用"。
- 一定要传 `scope`。省略时会展开客户端的全部 scope，若客户端没关"允许离线请求"就会拿到 `refresh_token`。
- 只对你自己已经认证过的用户签断言。

---

## 4. 常见错误

失败时返回 `400 invalid_grant`，`error_description` 故意写得笼统（以下为中文环境的提示语）。**真正原因看 SSO 日志**里的 `Domain assertion rejected ... reason:`，同一行还会打印 `registeredKeys`（已登记的公钥数）和 `expectedAudiences`（SSO 接受的 `aud`）。

| 返回 | 常见原因 |
| --- | --- |
| `invalid_client` | `client_id` 或 `client_secret` 错。`client_secret` 要用明文，不是后台显示的哈希 |
| `unauthorized_client` | 客户端没勾选 jwt-bearer 授权方式 |
| 域身份断言无效或已过期 | `aud` 与 SSO 不一致；断言已过期或 `exp - iat` 超过 300 秒；`iss` 不等于 `client_id`；缺 `jti`；签名或 `kid` 与登记的 JWK 对不上。日志里 `registeredKeys` 为 0，说明 JWK 没登记成功或已过期 |
| 域身份断言已被使用 | 同一份断言发了第二次，重新签 |
| 域账号 X 不存在或已被禁用 | AD 里没有该账号，或 AD / MASA 里已禁用（AD 状态最多延迟 60 秒生效） |
| 域账号 X 尚未在系统中建立账号 | 账号还没同步进 MASA，见第 1 节 |
| 域账号 X 不在该客户端允许断言的范围内 | 被 `domain_assertion.account_pattern` 挡住 |
| 域身份断言必须由已认证的客户端发起 | 客户端关闭了「需要客户端密钥」 |
| 域身份断言重放校验不可用，请稍后重试 | Redis 不可用 |
| `The authorization request could not be completed.` | SSO 访问 Auth 服务、AD 等依赖失败，不是断言的问题 |
