// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public sealed class CacheKey
{
    const string PERMISSION_CACHE_KEY_PRE = "permission:";
    const string ROLE_CACHE_KEY_PRE = "role:";
    const string USER_CACHE_KEY_PRE = "user:";
    const string USER_MENU_COLLECT_PRE = "menu_collect:";
    const string USER_VISIT_PRE = "user_visit:";

    const string USER_SYSTEM_DATA_PRE = "user_system_data:";

    public static string PermissionKey(Guid permissionId)
    {
        return $"{PERMISSION_CACHE_KEY_PRE}{permissionId}";
    }

    public static string RoleKey(Guid roleId)
    {
        return $"{ROLE_CACHE_KEY_PRE}{roleId}";
    }

    public static string UserKey(Guid userId)
    {
        return $"{USER_CACHE_KEY_PRE}{userId}";
    }

    public static string UserMenuCollectKey(Guid userId)
    {
        return $"{USER_MENU_COLLECT_PRE}{userId}";
    }

    public static string UserVisitKey(Guid userId)
    {
        return $"{USER_VISIT_PRE}{userId}";
    }

    public static string UserSystemDataKey(Guid userId, string systemId)
    {
        return $"{USER_SYSTEM_DATA_PRE}{userId}::{systemId}";
    }
}
