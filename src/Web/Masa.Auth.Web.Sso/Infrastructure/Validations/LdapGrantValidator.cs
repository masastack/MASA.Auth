// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class LdapGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    readonly IThirdPartyIdpService _thirdPartyIdpService;
    readonly ILdapFactory _ldapFactory;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.LDAP;

    public LdapGrantValidator(IAuthClient authClient, IThirdPartyIdpService thirdPartyIdpService, ILdapFactory ldapFactory, ILogger<LdapGrantValidator> logger)
        : base(authClient, logger)
    {
        _thirdPartyIdpService = thirdPartyIdpService;
        _ldapFactory = ldapFactory;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var userName = context.Request.Raw["userName"];
            if (string.IsNullOrEmpty(userName))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_USER_NAME);
            }

            var password = context.Request.Raw["password"] ?? string.Empty;

            var ldapOption = await _thirdPartyIdpService.GetLdapOptionsAsync(LdapConsts.LDAP_NAME);
            if (ldapOption is null)
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.NO_LDAP_CONFIG);
            }
            var ldapOptions = ldapOption.Adapt<LdapOptions>();
            var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);
            if (await ldapProvider.AuthenticateAsync(ldapOption.RootUserDn, ldapOption.RootUserPassword) is false)
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.LDAP_CONNECT_ERROR);
            }

            if (!string.IsNullOrEmpty(password))
            {
                var dc = new Regex("(?<=DC=).+(?=,)").Match(ldapOptions.BaseDn).Value;
                if (!await ldapProvider.AuthenticateAsync($"{dc}\\{userName}", password))
                {
                    throw new UserFriendlyException(errorCode: SsoErrorCodes.LDAP_USER_VALIDATE_FAILED, userName);
                }
            }

            var ldapUser = await ldapProvider.GetUserByUserNameAsync(userName);
            if (ldapUser is null)
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.LDAP_USER_NOT_EXIST);
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

            var claims = new List<Claim>
            {
                new Claim(IdentityClaimConsts.DOMAIN_NAME, ldapUser.SamAccountName)
            };

            context.Result = new GrantValidationResult(authUser.Id.ToString(), "ldap", claims);

            // Record token acquisition operation log (including client information)
            await RecordTokenOperationLogAsync(authUser, $"用户Token获取：使用LDAP账号{userName}获取访问Token", context.Request.Client?.ClientId, nameof(LdapGrantValidator));
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
