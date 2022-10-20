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
    const string MSG_CODE_FOR_UPDATE_USER_PHONENUMBER = "msg_code_update_user_phoneNumber:";
    const string MSG_CODE_FOR_LOGIN = "msg_code_login:";
    const string MSG_CODE_FOR_REGISTER = "msg_code_register:";
    const string MSG_CODE_FOR_VERIFIY_USER_PHONENUMBER = "msg_code_verifiy_user_phoneNumber:";
    const string VERIFIY_USER_PHONENUMBER_RESULT = "verifiy_user_phoneNumber_result:";
    const string USER_ELEMENT_PERMISSION_CODE_PRE = "user_element_permission_code:";
    const string EMAIL_REGISTER_CODE_PRE = "email_register_code:";
    const string EMAIL_REGISTER_SEND_PRE = "email_register_send:";
    const string SMS_REGISTER_SEND_PRE = "sms_register_send:";
    const string STAFF = "staff";
    public const string STAFF_DEFAULT_PASSWORD = "staff_default_password";

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

    public static string StaffKey(Guid staffId)
    {
        return $"{STAFF}{staffId}";
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
        return $"{USER_SYSTEM_DATA_PRE}{userId}:{systemId}";
    }

    public static string AccountLoginKey(string account)
    {
        return $"{ACCOUNT_LOGIN_PRE}{account}";
    }

    public static string MsgCodeForUpdateUserPhoneNumberKey(string userId, string phoneNumber)
    {
        return $"{MSG_CODE_FOR_UPDATE_USER_PHONENUMBER}{userId}{phoneNumber}";
    }

    public static string MsgCodeForLoginKey(string userId, string phoneNumber)
    {
        return $"{MSG_CODE_FOR_LOGIN}{userId}{phoneNumber}";
    }

    public static string MsgCodeForRegisterKey(string phoneNumber)
    {
        return $"{MSG_CODE_FOR_REGISTER}{phoneNumber}";
    }

    public static string EmailCodeRegisterKey(string email)
    {
        return $"{EMAIL_REGISTER_CODE_PRE}{email}";
    }

    public static string MsgCodeForRegisterSendKey(string phoneNumber)
    {
        return $"{EMAIL_REGISTER_SEND_PRE}{phoneNumber}";
    }

    public static string EmailCodeRegisterSendKey(string email)
    {
        return $"{SMS_REGISTER_SEND_PRE}{email}";
    }

    public static string MsgCodeForVerifiyUserPhoneNumberKey(string userId, string phoneNumber)
    {
        return $"{MSG_CODE_FOR_VERIFIY_USER_PHONENUMBER}{userId}{phoneNumber}";
    }

    public static string VerifiyUserPhoneNumberResultKey(string userId, string phoneNumber)
    {
        return $"{VERIFIY_USER_PHONENUMBER_RESULT}{userId}{phoneNumber}";
    }

    public static string UserElementPermissionCodeKey(Guid userId, string appId)
    {
        return $"{USER_ELEMENT_PERMISSION_CODE_PRE}{userId}:{appId}";
    }
}
