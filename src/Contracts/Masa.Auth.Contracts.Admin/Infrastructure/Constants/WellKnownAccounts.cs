// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

/// <summary>
/// 内置账号。内置管理员账号按不区分大小写匹配,防止 Admin 等变体绕过保护。
/// </summary>
public static class WellKnownAccounts
{
    public const string Admin = "admin";

    public static bool IsAdmin(string? account)
        => string.Equals(account, Admin, StringComparison.OrdinalIgnoreCase);
}
