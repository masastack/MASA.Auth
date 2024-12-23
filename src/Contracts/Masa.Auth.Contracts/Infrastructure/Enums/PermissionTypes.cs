// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Enums;

public enum PermissionTypes
{
    [Description("菜单权限")]
    Menu = 1,
    [Description("元素权限")]
    Element,
    [Description("API权限")]
    Api,
    [Description("数据权限")]
    Data
}
