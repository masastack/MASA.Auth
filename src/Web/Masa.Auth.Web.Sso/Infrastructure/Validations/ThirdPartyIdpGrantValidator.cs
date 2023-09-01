// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ThirdPartyIdpGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    IRemoteAuthenticationDefaultsProvider _remoteAuthenticationDefaultsProvider;
    ThirdPartyIdpCallerProvider _thirdPartyIdpCallerProvider;
    WeChatMiniProgramCaller _weChatMiniProgramCaller;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.THIRD_PARTY_IDP;

    public ThirdPartyIdpGrantValidator(
        IAuthClient authClient,
        IRemoteAuthenticationDefaultsProvider remoteAuthenticationDefaultsProvider,
        ThirdPartyIdpCallerProvider thirdPartyIdpCallerProvider,
        WeChatMiniProgramCaller weChatMiniProgramCaller)
    {
        _authClient = authClient;
        _remoteAuthenticationDefaultsProvider = remoteAuthenticationDefaultsProvider;
        _thirdPartyIdpCallerProvider = thirdPartyIdpCallerProvider;
        _weChatMiniProgramCaller = weChatMiniProgramCaller;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
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
        if (identity != null)
        {
            context.Result.CustomResponse = new()
            {
                ["thirdPartyUserData"] = identity,
                ["registerSuccess"] = user is not null
            };
        }
    }
}
