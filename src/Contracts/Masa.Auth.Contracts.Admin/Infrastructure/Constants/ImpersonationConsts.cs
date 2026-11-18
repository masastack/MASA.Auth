// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

/// <summary>
/// 模拟登录（RFC 8693 Token Exchange）在 SSO 与 Admin 之间共用的协议常量。
/// 标准 URN（如 access_token 类型、请求参数名）请直接用 IdentityModel 的 <c>OidcConstants</c>，这里只放 MASA 自定义的部分。
/// </summary>
public static class ImpersonationConsts
{
    /// <summary>自定义 <c>subject_token_type</c>：<c>subject_token</c> 为目标用户 id 字符串（授权来自 <c>actor_token</c>）。</summary>
    public const string SubjectTokenTypeUserId = "urn:masa:params:oauth:token-type:user-id";
}
