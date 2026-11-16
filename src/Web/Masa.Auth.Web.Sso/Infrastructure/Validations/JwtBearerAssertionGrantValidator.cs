// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Config;

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// RFC 7523 域身份断言：调用方用私钥签短命 JWT（sub=域账号）；client_secret 认证与 assertion 验签（<see cref="DomainAssertionValidator"/>）相互独立。
/// jti 单次、aud 绑定 token endpoint、须回 AD/MASA 复核；域断言客户端建议关闭 AllowOfflineAccess（<see cref="TokenExchangeGrantValidator"/> 模拟登录另策）。
/// </summary>
public class JwtBearerAssertionGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    readonly DomainAccountResolver _accountResolver;
    readonly JtiReplayCache _jtiReplayCache;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IMasaStackConfig _masaStackConfig;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.JWT_BEARER;

    public JwtBearerAssertionGrantValidator(
        IAuthClient authClient,
        DomainAccountResolver accountResolver,
        JtiReplayCache jtiReplayCache,
        IHttpContextAccessor httpContextAccessor,
        IMasaStackConfig masaStackConfig,
        ILogger<JwtBearerAssertionGrantValidator> logger)
        : base(authClient, logger)
    {
        _accountResolver = accountResolver;
        _jtiReplayCache = jtiReplayCache;
        _httpContextAccessor = httpContextAccessor;
        _masaStackConfig = masaStackConfig;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var client = context.Request.Client;

        try
        {
            // 公开客户端无 client_secret 保障，与 token-exchange 一致一律拒绝
            if (!client.RequireClientSecret)
            {
                _logger.LogWarning(
                    "Domain assertion rejected: client {ClientId} is a public client (RequireClientSecret=false)",
                    client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_CLIENT_REQUIRED);
            }

            var assertion = context.Request.Raw[OidcConstants.TokenRequest.Assertion];
            var now = DateTime.UtcNow;
            var signingKeys = DomainAssertionValidator.BuildSigningKeys(client.ClientSecrets, now);
            var validation = DomainAssertionValidator.Validate(
                assertion,
                client.ClientId,
                signingKeys,
                BuildExpectedAudiences(context),
                now);
            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "Domain assertion rejected. clientId:{ClientId}, reason:{Reason}",
                    client.ClientId, validation.Error);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_INVALID);
            }

            // sub 进 LDAP 过滤器前须挡元字符；账号白名单未配时不替代此检查
            var subject = validation.Principal!.FindFirst(JwtClaimTypes.Subject)?.Value;
            var account = DomainAccountName.Normalize(subject);
            if (!DomainAccountName.IsValid(account))
            {
                _logger.LogWarning(
                    "Domain assertion subject rejected. clientId:{ClientId}, reason:invalid account name",
                    client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_INVALID);
            }

            // jti 单次性（重放防护）
            if (!await _jtiReplayCache.TryRegisterAsync(
                    client.ClientId, validation.Jti!, validation.ExpiresUtc!.Value, TimeSpan.FromSeconds(DomainAssertionValidator.ClockSkewSeconds)))
            {
                _logger.LogWarning(
                    "Domain assertion replay detected. clientId:{ClientId}, jti:{Jti}",
                    client.ClientId, validation.Jti);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_REPLAYED);
            }

            // 可选账号 pattern 收敛
            EnsureAccountAllowed(client, account);

            // 回 AD 复核存在性与禁用态
            var resolved = await _accountResolver.ResolveAsync(account);
            if (resolved is null || resolved.IsDisabled)
            {
                _logger.LogWarning(
                    "Domain assertion target rejected. clientId:{ClientId}, account:{Account}, exists:{Exists}, disabled:{Disabled}",
                    client.ClientId, account, resolved is not null, resolved?.IsDisabled);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_ACCOUNT_REJECTED, account);
            }

            var authUser = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
            {
                ThirdPartyIdpType = ThirdPartyIdpTypes.Ldap,
                ThridPartyIdentity = resolved.ObjectGuid
            });
            if (authUser is null)
            {
                throw new UserFriendlyException(
                    errorCode: SsoErrorCodes.DOMAIN_ASSERTION_ACCOUNT_NOT_PROVISIONED, account);
            }

            // MASA 禁用/删除不可凭断言绕过
            if (!authUser.Enabled || authUser.IsDeleted)
            {
                _logger.LogWarning(
                    "Domain assertion target rejected. clientId:{ClientId}, account:{Account}, masaEnabled:{Enabled}, masaDeleted:{Deleted}",
                    client.ClientId, account, authUser.Enabled, authUser.IsDeleted);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_ACCOUNT_REJECTED, account);
            }

            var claims = new List<Claim>
            {
                new(IdentityClaimConsts.DOMAIN_NAME, resolved.Account)
            };

            // amr 由 authenticationMethod 写入，勿在 claims 再挂一条（会重复 amr）
            context.Result = new GrantValidationResult(
                authUser.Id.ToString(),
                OidcConstants.AuthenticationMethods.WindowsIntegratedAuthentication,
                claims);

            // 免密通道须记审计日志
            await RecordTokenOperationLogAsync(
                authUser,
                $"用户Token获取：客户端{client.ClientId}使用域身份断言为账号{account}获取访问Token",
                client.ClientId,
                nameof(JwtBearerAssertionGrantValidator));
        }
        catch (Exception ex)
        {
            // 业务拒绝 Warning；基础设施故障 Error；对外 error_description 不泄露细节
            _logger.Log(ex is UserFriendlyException ? LogLevel.Warning : LogLevel.Error, ex,
                "Extension grant {GrantType} rejected. clientId:{ClientId}, error:{ErrorMessage}",
                GrantType, client.ClientId, ex.Message);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ToErrorDescription(ex));
        }
    }

    /// <summary>优先 Stack SSO 域名与 IssuerUri；皆无时才用请求 Host（仅本地）。有配置时拒伪造 Host。</summary>
    IReadOnlyCollection<string> BuildExpectedAudiences(ExtensionGrantValidationContext context)
    {
        var audiences = DomainAssertionValidator.BuildAudiences(
            _masaStackConfig.GetSsoDomain(),
            context.Request.Options?.IssuerUri);
        if (audiences.Count > 0)
        {
            return audiences;
        }

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request is null)
        {
            return audiences;
        }

        _logger.LogWarning(
            "Neither the MASA Stack SSO domain nor the IdentityServer IssuerUri is available, " +
            "deriving the assertion audience from the request host. This is only acceptable in local development.");
        return DomainAssertionValidator.BuildAudiences($"{request.Scheme}://{request.Host}{request.PathBase}");
    }

    static void EnsureAccountAllowed(Client client, string account)
    {
        var pattern = GetProperty(client, DomainAccountMatching.AccountPatternProperty);

        // 未配 pattern 则不限制（边界退化为 AD 存在性）
        if (!DomainAccountMatching.IsAllowed(pattern, account))
        {
            throw new UserFriendlyException(
                errorCode: SsoErrorCodes.DOMAIN_ASSERTION_ACCOUNT_OUT_OF_SCOPE, account);
        }
    }

    /// <summary>按忽略大小写读 Client.Properties；勿对 IDictionary 用 FirstOrDefault（KVP 空判断失效）。</summary>
    internal static string? GetProperty(Client client, string key)
    {
        if (client.Properties is null)
        {
            return null;
        }

        foreach (var pair in client.Properties)
        {
            if (string.Equals(pair.Key, key, StringComparison.OrdinalIgnoreCase))
            {
                return pair.Value;
            }
        }

        return null;
    }
}
