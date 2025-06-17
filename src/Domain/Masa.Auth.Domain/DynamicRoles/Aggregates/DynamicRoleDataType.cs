// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class DynamicRoleDataType : Enumeration
{
    public static DynamicRoleDataType Default = new DynamicRoleDataType();

    public static DynamicRoleDataType UserInfo = new DynamicRoleDataType(1, nameof(UserInfo));

    public static DynamicRoleDataType UserClaim = new DynamicRoleDataType(2, nameof(UserClaim));

    public static DynamicRoleDataType DynamicRole = new DynamicRoleDataType(3, nameof(DynamicRole));

    public DynamicRoleDataType() : base(0, "") { }

    public DynamicRoleDataType(int id, string name) : base(id, name)
    {
    }
}
