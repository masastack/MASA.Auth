// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

/// <summary>
/// 管理后台模拟登录：以调用管理员的 access_token 作为 actor_token，向 SSO 发起 RFC 8693 token-exchange，
/// 换取目标用户的短命 access_token（sub=目标、act=操作者，≤900s、无 refresh_token）。
/// </summary>
public record ImpersonateTokenCommand(Guid UserId) : Command
{
    public ImpersonateTokenOutput Result { get; set; } = new();
}
