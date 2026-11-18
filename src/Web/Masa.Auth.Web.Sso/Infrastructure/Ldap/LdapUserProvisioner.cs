// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Ldap.Novell.Entries;

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// 按 AD <c>ObjectGuid</c> 获取或创建 MASA 用户（密码 grant 可首开；JWT 断言不开户）。绑定键用 ObjectGuid 而非 sAMAccountName。
/// </summary>
public static class LdapUserProvisioner
{
    public static async Task<UserModel> GetOrCreateUserAsync(IAuthClient authClient, LdapUser ldapUser)
    {
        var authUser = await authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
        {
            ThirdPartyIdpType = ThirdPartyIdpTypes.Ldap,
            ThridPartyIdentity = ldapUser.ObjectGuid
        });

        if (authUser is not null)
        {
            return authUser;
        }

        return await authClient.UserService.AddThirdPartyUserAsync(new AddThirdPartyUserModel
        {
            Scheme = LdapConsts.LDAP_NAME,
            ThridPartyIdentity = ldapUser.ObjectGuid,
            ExtendedData = ldapUser,
            User = new AddUserModel
            {
                Name = ldapUser.Name,
                DisplayName = ldapUser.DisplayName,
                Email = ldapUser.EmailAddress,
                PhoneNumber = ldapUser.Phone,
                Account = ldapUser.SamAccountName
            }
        });
    }
}
