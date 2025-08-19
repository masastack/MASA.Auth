// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ThirdPartyIdpGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    IRemoteAuthenticationDefaultsProvider _remoteAuthenticationDefaultsProvider;
    ThirdPartyIdpCallerProvider _thirdPartyIdpCallerProvider;
    WeChatMiniProgramCaller _weChatMiniProgramCaller;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.THIRD_PARTY_IDP;

    public ThirdPartyIdpGrantValidator(
        IAuthClient authClient,
        IRemoteAuthenticationDefaultsProvider remoteAuthenticationDefaultsProvider,
        ThirdPartyIdpCallerProvider thirdPartyIdpCallerProvider,
        WeChatMiniProgramCaller weChatMiniProgramCaller,
        ILogger<ThirdPartyIdpGrantValidator> logger)
        : base(authClient, logger)
    {
        _remoteAuthenticationDefaultsProvider = remoteAuthenticationDefaultsProvider;
        _thirdPartyIdpCallerProvider = thirdPartyIdpCallerProvider;
        _weChatMiniProgramCaller = weChatMiniProgramCaller;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var scheme = context.Request.Raw["scheme"];
            var code = context.Request.Raw["code"];
            var idToken = context.Request.Raw["idToken"];
            var wechatMini = context.Request.Raw["wechat_mini"];//ogin by wechat mini program,temporary addition

            if (string.IsNullOrEmpty(scheme))
            {
                throw new UserFriendlyException("must provider scheme");
            }
            var authenticationDefaults = await _remoteAuthenticationDefaultsProvider.GetAsync(scheme) ?? throw new UserFriendlyException($"No {scheme} configuration information found");
            Security.OAuth.Providers.Identity? identity = null;

            string? thridPartyIdentity;
            if (bool.TryParse(wechatMini, out bool mini) && mini)
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new UserFriendlyException("must provider code");
                }
                var options = new OAuthOptions();
                authenticationDefaults.BindOAuthOptions(options);
                thridPartyIdentity = await _weChatMiniProgramCaller.GetOpenIdAsync(options, code);
                // 为微信小程序场景构建最小可用的 Identity，保证下方 CustomResponse 正常返回
                if (!string.IsNullOrWhiteSpace(thridPartyIdentity))
                {
                    identity = new Security.OAuth.Providers.Identity(thridPartyIdentity)
                    {
                        Issuer = authenticationDefaults.Scheme
                    };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(code))
                {
                    identity = await _thirdPartyIdpCallerProvider.GetIdentity(authenticationDefaults, code);
                }
                else
                {
                    if (string.IsNullOrEmpty(idToken))
                    {
                        throw new UserFriendlyException("must provider code or idToken");
                    }
                    identity = await _thirdPartyIdpCallerProvider.GetIdentityByIdToken(authenticationDefaults, idToken);
                }
                thridPartyIdentity = identity.Subject;
            }

            var user = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
            {
                ThridPartyIdentity = thridPartyIdentity
            });

            context.Result = new GrantValidationResult(user?.Id.ToString() ?? "", "thirdPartyIdp");
            context.Result.CustomResponse = new()
            {
                ["thirdPartyUserData"] = identity,
                ["registerSuccess"] = user is not null
            };

            // If user exists, record token acquisition operation log
            if (user != null)
            {
                await RecordTokenOperationLogAsync(user, $"用户Token获取：使用{scheme}第三方登录获取访问Token", context.Request.Client?.ClientId, nameof(ThirdPartyIdpGrantValidator));
            }
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
