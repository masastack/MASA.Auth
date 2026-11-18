// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Constants;

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>用配置的 root 账号建立 AD 连接，统一取 LDAP 配置、创建 provider 与 bind。</summary>
public static class LdapRootConnection
{
    /// <summary>已 root bind 的 provider 及对应 LDAP 配置。</summary>
    public sealed record Session(ILdapProvider Provider, LdapOptions Options);

    /// <exception cref="UserFriendlyException">
    /// <see cref="SsoErrorCodes.NO_LDAP_CONFIG"/>：未配置 LDAP；
    /// <see cref="SsoErrorCodes.LDAP_CONNECT_ERROR"/>：root 账号 bind 失败。
    /// </exception>
    public static async Task<Session> OpenAsync(IThirdPartyIdpService thirdPartyIdpService, ILdapFactory ldapFactory)
    {
        var ldapOption = await thirdPartyIdpService.GetLdapOptionsAsync(LdapConsts.LDAP_NAME);
        if (ldapOption is null)
        {
            throw new UserFriendlyException(errorCode: SsoErrorCodes.NO_LDAP_CONFIG);
        }

        var options = ldapOption.Adapt<LdapOptions>();
        var provider = ldapFactory.CreateProvider(options);
        if (!await provider.AuthenticateAsync(ldapOption.RootUserDn, ldapOption.RootUserPassword))
        {
            throw new UserFriendlyException(errorCode: SsoErrorCodes.LDAP_CONNECT_ERROR);
        }

        return new Session(provider, options);
    }
}
