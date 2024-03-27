// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Psso;

public class GetPermissionsByLonsidUserIdOutput
{
    /// <summary>
    /// 用户id
    /// </summary>
    public Guid? LonsidUserId { set; get; }
    /// <summary>
    /// 授权权限集合
    /// </summary>
    public List<string> Permission { set; get; } = new();
    /// <summary>
    /// 用户角色
    /// </summary>
    public List<AssignedRoleDto> AssignedRoles { get; set; } = new();
}