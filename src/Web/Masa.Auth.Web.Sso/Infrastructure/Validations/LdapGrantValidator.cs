// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// <c>grant_type=ldap</c>：AD 账号+密码经 bind 换 token；无密码拒绝。域身份断言走 <see cref="JwtBearerAssertionGrantValidator"/>。
/// </summary>
public class LdapGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    readonly IThirdPartyIdpService _thirdPartyIdpService;
    readonly ILdapFactory _ldapFactory;
    readonly LdapLoginThrottle _loginThrottle;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.LDAP;

    public LdapGrantValidator(
        IAuthClient authClient,
        IThirdPartyIdpService thirdPartyIdpService,
        ILdapFactory ldapFactory,
        LdapLoginThrottle loginThrottle,
        ILogger<LdapGrantValidator> logger)
        : base(authClient, logger)
    {
        _thirdPartyIdpService = thirdPartyIdpService;
        _ldapFactory = ldapFactory;
        _loginThrottle = loginThrottle;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var userName = context.Request.Raw["userName"];
        // 仅凭据类失败计入限流，基础设施故障不计（否则 LDAP 宕机会冻结全员）
        var credentialFailure = false;

        try
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_USER_NAME);
            }

            // 非法 userName 拦截 LDAP 注入；统一凭据错误文案；不计限流
            if (!DomainAccountName.IsValid(userName))
            {
                throw InvalidCredentials(userName);
            }

            // 空密码必须在此拒绝且仍执行 bind 路径不可被跳过，否则存在账号可免密换 token
            var password = context.Request.Raw["password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                credentialFailure = true;
                throw InvalidCredentials(userName);
            }

            await _loginThrottle.EnsureNotFrozenAsync(userName);

            var session = await LdapRootConnection.OpenAsync(_thirdPartyIdpService, _ldapFactory);
            var ldapProvider = session.Provider;

            var dc = GetNetBiosDomain(session.Options.BaseDn);
            if (string.IsNullOrEmpty(dc))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.NO_LDAP_CONFIG);
            }

            if (!await ldapProvider.AuthenticateAsync($"{dc}\\{userName}", password))
            {
                credentialFailure = true;
                throw InvalidCredentials(userName);
            }

            var ldapUser = await ldapProvider.GetUserByUserNameAsync(userName);
            // 不存在/禁用与密码错同源错误，防账号枚举；细节见日志
            if (ldapUser is null || ldapUser.IsAccountDisabled)
            {
                credentialFailure = true;
                throw InvalidCredentials(userName);
            }

            var authUser = await LdapUserProvisioner.GetOrCreateUserAsync(_authClient, ldapUser);

            // MASA 禁用/删除不可凭 AD 密码绕过；对外仍用统一凭据错误
            if (!authUser.Enabled || authUser.IsDeleted)
            {
                credentialFailure = true;
                throw InvalidCredentials(userName);
            }

            var claims = new List<Claim>
            {
                new Claim(IdentityClaimConsts.DOMAIN_NAME, ldapUser.SamAccountName)
            };

            context.Result = new GrantValidationResult(authUser.Id.ToString(), "ldap", claims);

            await _loginThrottle.ResetAsync(userName);

            // Record token acquisition operation log (including client information)
            await RecordTokenOperationLogAsync(authUser, $"用户Token获取：使用LDAP账号{userName}获取访问Token", context.Request.Client?.ClientId, nameof(LdapGrantValidator));
        }
        catch (Exception ex)
        {
            if (credentialFailure)
            {
                await _loginThrottle.RecordFailureAsync(userName);
            }

            // 业务拒绝对 Warning，基础设施对 Error；对外 error_description 不泄露内部细节
            _logger.Log(ex is UserFriendlyException ? LogLevel.Warning : LogLevel.Error, ex,
                "Extension grant {GrantType} rejected. userName:{UserName}, clientId:{ClientId}, error:{ErrorMessage}",
                GrantType, userName, context.Request.Client?.ClientId, ex.Message);
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ToErrorDescription(ex));
        }
    }

    /// <summary>取 BaseDn 第一个 <c>DC=</c> 分量作 NetBIOS 域名（多级域时贪婪正则会拼错 bind 名）。</summary>
    public static string? GetNetBiosDomain(string? baseDn)
    {
        if (string.IsNullOrWhiteSpace(baseDn))
        {
            return null;
        }

        var match = Regex.Match(baseDn, @"(?:^|,)\s*DC=(?<dc>[^,]+)", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        return match.Success ? match.Groups["dc"].Value.Trim() : null;
    }

    /// <summary>统一凭据无效文案，防 AD 账号枚举。</summary>
    static UserFriendlyException InvalidCredentials(string? userName)
        => new(errorCode: SsoErrorCodes.LDAP_USER_VALIDATE_FAILED, userName ?? string.Empty);
}
