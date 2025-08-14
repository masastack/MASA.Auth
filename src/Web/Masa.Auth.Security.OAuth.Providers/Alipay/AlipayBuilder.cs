// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Alipay;

/// <summary>
/// 支付宝OAuth认证构建器
/// </summary>
public class AlipayBuilder : ILocalAuthenticationDefaultBuilder, IAuthenticationInject
{
    public string Scheme { get; } = "Alipay";

    public AuthenticationDefaults AuthenticationDefaults { get; } = new AuthenticationDefaults
    {
        HandlerType = typeof(AlipayAuthenticationHandler),
        Scheme = "Alipay",
        DisplayName = AlipayAuthenticationDefaults.DisplayName,
        Icon = "https://cdn.masastack.com/stack/auth/ico/alipay.svg",
        CallbackPath = AlipayAuthenticationDefaults.CallbackPath,
        Issuer = AlipayAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = AlipayAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = AlipayAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = AlipayAuthenticationDefaults.UserInformationEndpoint,
        ThirdPartyIdpType = ThirdPartyIdpTypes.Alipay,
        JsonKeyMap = new Dictionary<string, string>
        {
            [UserClaims.Subject] = "user_id",
            [UserClaims.NickName] = "nick_name",
            [UserClaims.Gender] = "gender",
            [UserClaims.Region] = "province",
            [UserClaims.Locale] = "city",
            [UserClaims.Picture] = "avatar"
        }
    };

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddAlipay(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
        });
    }

    public void InjectForHotUpdate(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AlipayAuthenticationOptions>, OAuthPostConfigureOptions<AlipayAuthenticationOptions, AlipayAuthenticationHandler>>());
    }
}
