// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class LdapGrantValidator : IExtensionGrantValidator
{
    readonly IAuthClient _authClient;
    readonly IThirdPartyIdpService _thirdPartyIdpService;
    readonly ILdapFactory _ldapFactory;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.LDAP;

    public LdapGrantValidator(IAuthClient authClient, IThirdPartyIdpService thirdPartyIdpService, ILdapFactory ldapFactory)
    {
        _authClient = authClient;
        _thirdPartyIdpService = thirdPartyIdpService;
        _ldapFactory = ldapFactory;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var userName = context.Request.Raw["userName"];
        if (string.IsNullOrEmpty(userName))
        {
            throw new UserFriendlyException("must provider userName");
        }

        var ldapOption = await _thirdPartyIdpService.GetLdapOptionsAsync(BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.LDAP);
        if (ldapOption is null)
        {
            throw new UserFriendlyException($"Not find ldap");
        }
        var ldapProvider = _ldapFactory.CreateProvider(ldapOption.Adapt<LdapOptions>());
        if (await ldapProvider.AuthenticateAsync(ldapOption.RootUserDn, ldapOption.RootUserPassword) is false)
        {
            throw new UserFriendlyException($"Ldap connect error");
        }
        var ldapUser = await ldapProvider.GetUserByUserNameAsync(userName);
        if (ldapUser is null)
        {
            throw new UserFriendlyException($"Ldap does not exist this user");
        }
        var authUser = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
        {
            ThirdPartyIdpType = ThirdPartyIdpTypes.Ldap,
            ThridPartyIdentity = ldapUser.ObjectGuid
        });
        if (authUser is null)
        {
            authUser = await _authClient.UserService.AddThirdPartyUserAsync(new AddThirdPartyUserModel
            {
                Scheme = "Ldap",
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
        context.Result = new GrantValidationResult(authUser.Id.ToString(), "ldap");
    }
}
