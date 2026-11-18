// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Models;

/// <summary>
/// 模拟登录代理所用的机密客户端凭据：token-exchange 只接受机密客户端，WASM 前端不能持有 secret，故由本服务端代理。
/// 凭据对应 SSO 管理员按 docs/sso-impersonation.md 2.1 手工创建的 Machine 客户端，secret 走配置不入种子。
/// 配置来源：仅 DCC auth-service 应用下的 JSON 配置对象 <c>Impersonation</c>（appsettings/环境变量不生效，见 Program.cs）。
/// 两项任一为空即视为功能未启用。
/// </summary>
public class ImpersonationClientOptions
{
    public const string SectionName = "Impersonation";

    /// <summary><see cref="Scope"/> 留空时使用的默认 scope（与此前写死的行为一致，存量部署不受影响）。</summary>
    public const string DefaultScope = "openid profile";

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    /// <summary>
    /// token-exchange 请求的 scope（空格分隔），应是该 Machine 客户端被授予的 scope 的子集，
    /// 并覆盖目标应用向 Caller 要的 scope（模拟 token 不能再换取更多 scope）。
    /// 不能含 <c>offline_access</c>（SSO 会拒绝）。留空则用 <see cref="DefaultScope"/>。
    /// </summary>
    public string? Scope { get; set; }
}
