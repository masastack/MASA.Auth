// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global;

/// <summary>
/// SSO 本地错误码（仅用于 connect/token grant validator 的硬编码消息多语言化），
/// 与 <see cref="Masa.Auth.Contracts.Admin.UserFriendlyExceptionCodes"/>（Auth 服务端错误码）互不影响。
/// 每个错误码对应 Assets/I18n/{zh-CN,en-US,ru-RU,ja-JP}.json 中的同名资源 key。
/// </summary>
public static class SsoErrorCodes
{
    public const string MUST_PROVIDER_EMAIL_AND_CODE = "MustProviderEmailAndCode";
    public const string MUST_PROVIDER_PHONE_NUMBER_AND_CODE = "MustProviderPhoneNumberAndCode";
    public const string INVALID_CUSTOM_CREDENTIAL = "InvalidCustomCredential";

    public const string MUST_PROVIDER_USER_NAME = "MustProviderUserName";
    public const string NO_LDAP_CONFIG = "NoLdapConfig";
    public const string LDAP_CONNECT_ERROR = "LdapConnectError";
    public const string LDAP_USER_VALIDATE_FAILED = "LdapUserValidateFailed";
    /// <summary>已合并入 <see cref="LDAP_USER_VALIDATE_FAILED"/>；常量仅保留 i18n 兼容。</summary>
    public const string LDAP_USER_NOT_EXIST = "LdapUserNotExist";

    public const string MUST_PROVIDER_SCHEME = "MustProviderScheme";
    public const string NO_IDP_CONFIG_FOUND = "NoIdpConfigFound";
    public const string MUST_PROVIDER_CODE = "MustProviderCode";
    public const string MUST_PROVIDER_CODE_OR_ID_TOKEN = "MustProviderCodeOrIdToken";

    public const string MUST_PROVIDER_PHONE_NUMBER_AND_SP_TOKEN = "MustProviderPhoneNumberAndSpToken";
    public const string PHONE_NUMBER_USER_NOT_EXIST = "PhoneNumberUserNotExist";

    // ---- RFC 7523 域身份断言（grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer） ----
    public const string DOMAIN_ASSERTION_CLIENT_REQUIRED = "DomainAssertionClientRequired";
    public const string DOMAIN_ASSERTION_INVALID = "DomainAssertionInvalid";
    public const string DOMAIN_ASSERTION_REPLAYED = "DomainAssertionReplayed";
    public const string DOMAIN_ASSERTION_ACCOUNT_REJECTED = "DomainAssertionAccountRejected";
    public const string DOMAIN_ASSERTION_ACCOUNT_NOT_PROVISIONED = "DomainAssertionAccountNotProvisioned";
    public const string DOMAIN_ASSERTION_ACCOUNT_OUT_OF_SCOPE = "DomainAssertionAccountOutOfScope";
    public const string DOMAIN_ASSERTION_REPLAY_CHECK_UNAVAILABLE = "DomainAssertionReplayCheckUnavailable";

    // ---- RFC 8693 Token Exchange（grant_type=urn:ietf:params:oauth:grant-type:token-exchange） ----
    public const string MUST_PROVIDER_ACTOR_TOKEN = "MustProviderActorToken";
    public const string INVALID_ACTOR_TOKEN = "InvalidActorToken";
    public const string IMPERSONATION_DISABLED = "ImpersonationDisabled";
    public const string IMPERSONATION_NOT_AUTHORIZED = "ImpersonationNotAuthorized";
    public const string IMPERSONATION_CHAIN_NOT_ALLOWED = "ImpersonationChainNotAllowed";
    public const string MUST_PROVIDER_SUBJECT_TOKEN = "MustProviderSubjectToken";
    public const string IMPERSONATION_TARGET_NOT_FOUND = "ImpersonationTargetNotFound";
    public const string IMPERSONATION_OFFLINE_ACCESS_NOT_ALLOWED = "ImpersonationOfflineAccessNotAllowed";
    public const string INVALID_TOKEN_TYPE = "InvalidTokenType";
}
