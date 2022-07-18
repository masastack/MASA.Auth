// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public sealed class CacheKey
{
    //todo method
    public const string PERMISSION_CACHE_KEY_PRE = "permission:";

    public const string ROLE_CACHE_KEY_PRE = "role:";

    public const string USER_CACHE_KEY_PRE = "user:";

    public const string USER_MENU_COLLECT_PRE = "menu_collect:";

    public const string USER_VISIT_PRE = "user_visit:";

    const string USER_SYSTEM_DATA_PRE = "user_system_data:";

    public static string PermissionKey(Guid permissionId)
    {
        return $"{CacheKey.PERMISSION_CACHE_KEY_PRE}{permissionId}";
    }

    public static string UserSystemDataKey(Guid userId, string systemId)
    {
        return $"{CacheKey.USER_SYSTEM_DATA_PRE}{userId}::{systemId}";
    }
}
