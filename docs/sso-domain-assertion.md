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
- 断言通道**不会**替账号开户。账号还没入库会被拒绝。

**接入流程**

| 步骤 | 谁做 | 做什么 |
| --- | --- | --- |
| 1 | 接入方 | 生成密钥对，把**公钥 JWK** 交给 SSO 管理员（2.1） |
| 2 | SSO 管理员 | 建客户端，登记 `SharedSecret` 和 JWK；把 `client_id` 和 `client_secret` 交给接入方（2.2） |
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

### 2.2 SSO 管理员：配置客户端

| 配置项 | 填什么 |
| --- | --- |
| 客户端类型 | **`Machine`**（新建时选择。其它类型会被拒绝） |
| 授权方式 | 勾选 `JwtBearer (RFC 7523, domain assertion)`。`Machine` 默认只有 `client_credentials`，需自己再勾 |
| 客户端密钥（共 2 条） | ① `SharedSecret`：设一个密钥，明文交给接入方作为 `client_secret`<br>② `JWK`：类型选 `JWK`，粘贴接入方给的公钥 JSON |
| Scope | 只给必要的 |
| 允许离线请求（`AllowOfflineAccess`） | **关闭** |
| 客户端属性 `domain_assertion.account_pattern` | **强烈建议配**，限定这个应用能断言哪些账号。`\|` 分隔多个模式，支持 `*` 和 `?`，不区分大小写，整串匹配。例：`svc_*\|zhang?`。不配则可断言 AD 里任意账号 |

JWK 保存时会校验，不通过会提示原因（不回显内容）：

- 必须是**单个**公钥，不接受 JWK Set。
- `kty` 为 `RSA` 或 `EC`，**必须带 `kid`**。
- 不能含私钥参数（`d`、`p`、`q` 等）。
- RSA 至少 2048 位，EC 曲线只接受 P-256 / P-384 / P-521。

也可以登记 `X509CertificateBase64`：公钥证书的 DER base64（不带 PEM 头，不接受 PFX）。`X509Thumbprint` 和 `X509Name` 不参与断言验签。

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
| payload | `sub` | 域账号。`mayue`、`LONSID\mayue`、`mayue@lonsid.cn` 都可以 |
| payload | `aud` | **`<SSO 对外域名>/connect/token`**，不带 `/connect/token` 的裸域名也接受 |
| payload | `iat`、`exp` | 必填。`exp - iat` 不超过 300 秒，建议 60 秒 |
| payload | `jti` | 必填，**每次都生成新值**，只能用一次 |
| payload | `nbf` | 可选 |

`aud` 里的"SSO 对外域名"就是 Stack 配置（`MASA_STACK`）里 SSO 的域名，不是你请求时用的内网地址或 `localhost`。本地调试时请求发到 `localhost`，`aud` 也仍然写这个域名。

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

**成功时**返回 `access_token`。token 里 `sub` 是 MASA 用户 Id（不是域账号），带 `domain_name`，`amr` 为 `["wia"]`，不带 `refresh_token`。token 过期后用**新断言**重新换。

**使用要点**：

- 每次请求都重新签一个断言，不要复用。
- 一定要传 `scope`。省略时会展开客户端的全部 scope，若客户端没关"允许离线请求"就会拿到 `refresh_token`。
- 只对你自己已经认证过的用户签断言。
