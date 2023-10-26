// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class CacheKey
{
    const string ALL_PERMISSION_CACHE_PRE = "all_permissions";
    const string ROLE_CACHE_PRE = "role:";
    const string USER_CACHE_PRE = "user:";
    const string USER_MENU_COLLECT_PRE = "menu_collect:";
    const string USER_VISIT_PRE = "user_visit:";
    const string ACCOUNT_LOGIN_PRE = "account_login:";
    const string USER_SYSTEM_DATA_PRE = "user_system_data:";
    const string MSG_CODE_FOR_UPDATE_USER_PHONENUMBER = "msg_code_update_user_phoneNumber:";
    const string MSG_CODE_REGISTER_LOGIN = "msg_code_register_and_login:";
    const string MSG_CODE_FOR_BIND = "msg_code_bind:";
    const string MSG_CODE_FOR_VERIFIY_USER_PHONENUMBER = "msg_code_verifiy_user_phoneNumber:";
    const string VERIFIY_USER_PHONENUMBER_RESULT = "verifiy_user_phoneNumber_result:";
    const string USER_API_PERMISSION_CODE_PRE = "user_api_permission_code:";
    const string EMAIL_REGISTER_CODE_PRE = "email_register_code:";
    const string EMAIL_BIND_CODE_PRE = "email_bind_code:";
    const string EMAIL_BIND_SEND_CODE_PRE = "email_bind_send_code:";
    const string EMAIL_REGISTER_SEND_PRE = "email_register_send:";
    const string EMAIL_VERIFIY_CODE_PRE = "email_verifiy_code:";
    const string EMAIL_VERIFIY_SEND_PRE = "email_verifiy_send:";
    const string EMAIL_FORGOT_PASSWORD_CODE_PRE = "email_forgot_password_code:";
    const string EMAIL_FORGOT_PASSWORD_SEND_PRE = "email_forgot_password_send:";
    const string MSG_CODE_FORGOT_PASSWORD_CODE_PRE = "msg_forgot_password_code:";
    const string MSG_CODE_FORGOT_PASSWORD_SEND_PRE = "msg_forgot_password_send:";
    const string EMAIL_UPDATE_CODE_PRE = "email_update_code:";
    const string EMAIL_UPDATE_SEND_PRE = "email_update_send:";
    public const string STAFF_DEFAULT_PASSWORD = "staff_default_password";
    const string MSG_VERIFIY_CODE_SEND_EXPIRED = "msg_verifiy_code_send_expired:";

    public static string AllPermissionKey()
    {
        return $"{ALL_PERMISSION_CACHE_PRE}";
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

    public static string MsgCodeRegisterAndLoginKey(string phoneNumber)
    {
        return $"{MSG_CODE_REGISTER_LOGIN}{phoneNumber}";
    }

    public static string MsgCodeForBindKey(string phoneNumber)
    {
        return $"{MSG_CODE_FOR_BIND}{phoneNumber}";
    }

    public static string EmailCodeRegisterSendKey(string email)
    {
        return $"{EMAIL_REGISTER_SEND_PRE}{email}";
    }

    public static string EmailCodeRegisterKey(string email)
    {
        return $"{EMAIL_REGISTER_CODE_PRE}{email}";
    }

    public static string EmailCodeBindKey(string email)
    {
        return $"{EMAIL_BIND_CODE_PRE}{email}";
    }

    public static string EmailCodeBindSendKey(string email)
    {
        return $"{EMAIL_BIND_SEND_CODE_PRE}{email}";
    }

    public static string EmailCodeVerifiySendKey(string email)
    {
        return $"{EMAIL_VERIFIY_SEND_PRE}{email}";
    }

    public static string EmailCodeVerifiyKey(string email)
    {
        return $"{EMAIL_VERIFIY_CODE_PRE}{email}";
    }

    public static string EmailCodeForgotPasswordSendKey(string email)
    {
        return $"{EMAIL_FORGOT_PASSWORD_SEND_PRE}{email}";
    }

    public static string EmailCodeForgotPasswordKey(string email)
    {
        return $"{EMAIL_FORGOT_PASSWORD_CODE_PRE}{email}";
    }

    public static string EmailCodeUpdateSendKey(string email)
    {
        return $"{EMAIL_UPDATE_SEND_PRE}{email}";
    }

    public static string EmailCodeUpdateKey(string email)
    {
        return $"{EMAIL_UPDATE_CODE_PRE}{email}";
    }

    public static string MsgCodeForgotPasswordSendKey(string phoneNumber)
    {
        return $"{MSG_CODE_FORGOT_PASSWORD_SEND_PRE}{phoneNumber}";
    }

    public static string MsgCodeForgotPasswordKey(string phoneNumber)
    {
        return $"{MSG_CODE_FORGOT_PASSWORD_CODE_PRE}{phoneNumber}";
    }

    public static string MsgCodeForVerifiyUserPhoneNumberKey(string userId, string phoneNumber)
    {
        return $"{MSG_CODE_FOR_VERIFIY_USER_PHONENUMBER}{userId}{phoneNumber}";
    }

    public static string VerifiyUserPhoneNumberResultKey(string userId, string phoneNumber)
    {
        return $"{VERIFIY_USER_PHONENUMBER_RESULT}{userId}{phoneNumber}";
    }

    public static string UserApiPermissionCodeKey(Guid userId, string appId)
    {
        return $"{USER_API_PERMISSION_CODE_PRE}{userId}:{appId}";
    }

    public static string MsgVerifiyCodeSendExpired(string key)
    {
        return $"{MSG_VERIFIY_CODE_SEND_EXPIRED}{key}";
    }
}
