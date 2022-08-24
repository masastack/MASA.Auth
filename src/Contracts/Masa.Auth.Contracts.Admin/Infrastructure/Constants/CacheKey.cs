// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class CacheKey
{
    const string PERMISSION_CACHE_PRE = "permission:";
    const string ROLE_CACHE_PRE = "role:";
    const string USER_CACHE_PRE = "user:";
    const string USER_MENU_COLLECT_PRE = "menu_collect:";
    const string USER_VISIT_PRE = "user_visit:";
    const string ACCOUNT_LOGIN_PRE = "account_login:";
    const string LDAP_OPTIONS_PRE = "ldap_options:";
    const string USER_SYSTEM_DATA_PRE = "user_system_data:";
    const string UPDATE_USER_PHONENUMBER_MSG_CODE = "update_user_phoneNumber_msg_code:";
    const string VERIFIY_USER_PHONENUMBER_MSG_CODE = "verifiy_user_phoneNumber_msg_code:";
    const string VERIFIY_USER_PHONENUMBER_MSG_CODE_RESULT = "verifiy_user_phoneNumber_msg_code_result:";

    public static string PermissionKey(Guid permissionId)
    {
        return $"{PERMISSION_CACHE_PRE}{permissionId}";
    }

    public static string RoleKey(Guid roleId)
    {
        return $"{ROLE_CACHE_PRE}{roleId}";
    }

    public static string UserKey(Guid userId)
    {
        return $"{USER_CACHE_PRE}{userId}";
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

    public static string AccountLoginKey(string account)
    {
        return $"{ACCOUNT_LOGIN_PRE}{account}";
    }

    public static string UpdateUserPhoneNumberKey(string userId, string phoneNumber)
    {
        return $"{UPDATE_USER_PHONENUMBER_MSG_CODE}{userId}{phoneNumber}";
    }

    public static string VerifiyUserPhoneNumberKey(string userId, string phoneNumber)
    {
        return $"{VERIFIY_USER_PHONENUMBER_MSG_CODE}{userId}{phoneNumber}";
    }

    public static string VerifiyUserPhoneNumberResultKey(string userId, string phoneNumber)
    {
        return $"{VERIFIY_USER_PHONENUMBER_MSG_CODE_RESULT}{userId}{phoneNumber}";
    }
}
