// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

/// <summary>
/// 管理后台模拟登录代理端点（POST api/user/impersonate-token）入参：目标用户 Id。
/// token 交换由 Service.Admin 持机密客户端凭据向 SSO 转发（RFC 8693 token-exchange）：
/// 调用管理员的 access_token 作为 actor_token，返回被模拟用户的短命 access_token（sub=目标用户、act=操作者）。
/// 注意：这不是已删除的旧版 grant_type=impersonation 的随机票据语义。
/// </summary>
public class ImpersonateInput
{
    public Guid UserId { get; set; }
}
