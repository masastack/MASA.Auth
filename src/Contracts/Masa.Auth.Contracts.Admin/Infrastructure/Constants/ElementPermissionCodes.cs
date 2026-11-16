// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class ElementPermissionCodes
{
    public static string TeamAdd { get; } = "team.add";
    public static string ApiTeamAdd { get; } = "api.team.create";
    public static string UserAccountUpdate = "user.account.update";

    /// <summary>模拟登录界面 Element 权限，经 BindApiPermission 关联 api.user.impersonate（同 <see cref="TeamAdd"/> 模式）。</summary>
    public static string UserImpersonate { get; } = "user.impersonate";

    /// <summary>模拟登录 API 权限；SSO 校验 actor 时使用。与 <see cref="UserImpersonate"/> 配对，同 <see cref="TeamAdd"/> / <see cref="ApiTeamAdd"/>。</summary>
    public static string ApiUserImpersonate { get; } = "api.user.impersonate";
}
