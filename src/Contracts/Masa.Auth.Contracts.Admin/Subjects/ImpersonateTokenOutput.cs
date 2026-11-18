// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

/// <summary>
/// 模拟登录代理端点出参：token-exchange 换得的短命 access_token（RFC 8693）。
/// sub=被模拟用户、act=发起管理员；最长 900 秒、无 refresh_token，到期即失效。
/// 目标应用据此建立模拟会话（接收端契约见 docs/sso-impersonation.md），会话到期自然结束。
/// </summary>
public class ImpersonateTokenOutput
{
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>token 有效期（秒）。</summary>
    public int ExpiresIn { get; set; }
}
