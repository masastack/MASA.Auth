// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AspNet.Security.OAuth.Weixin.WeixinAuthenticationConstants;

namespace Masa.Auth.Security.OAuth.Providers.WeChat;

public class WeChatBuilder : IIdentityBuilder, ILocalAuthenticationDefaultBuilder, IAuthenticationInject, IAuthenticationInstanceBuilder
{
    public string Scheme { get; } = "WeChat";

    public AuthenticationDefaults AuthenticationDefaults { get; } = new AuthenticationDefaults
    {
        HandlerType = typeof(WeixinAuthenticationHandler),
        Scheme = "WeChat",
        DisplayName = WeixinAuthenticationDefaults.DisplayName,
        Icon = "https://cdn.masastack.com/stack/auth/ico/wechat.svg",
        CallbackPath = WeixinAuthenticationDefaults.CallbackPath,
        Issuer = WeixinAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = WeixinAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = WeixinAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = WeixinAuthenticationDefaults.UserInformationEndpoint,
        JsonKeyMap = new Dictionary<string, string>
        {
            [UserClaims.Subject] = "unionid",
            [UserClaims.NickName] = "nickname",
            [UserClaims.Gender] = "sex",
            [UserClaims.Country] = "country",
            [UserClaims.Profile] = "openid",
            [UserClaims.Picture] = "headimgurl",
            [UserClaims.Locale] = "city",
            [UserClaims.Region] = "province",
        }
    };

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);

        return identity;
    }

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddWeixin(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
        });
    }

    public void InjectForHotUpdate(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WeixinAuthenticationOptions>, OAuthPostConfigureOptions<WeixinAuthenticationOptions, WeixinAuthenticationHandler>>());
    }

    public IAuthenticationHandler CreateInstance(IServiceProvider provider, AuthenticationDefaults authenticationDefaults)
    {
        var (options, loggerFactory, urlEncoder, systemClock) = CreateAuthenticationHandlerInstanceUtilities.BuilderParamter<WeixinAuthenticationOptions>(provider, authenticationDefaults.Scheme);
        authenticationDefaults.BindOAuthOptions(options.CurrentValue);
        options.CurrentValue.ClaimActions.MapCustomJson(Claims.Privilege, user =>
        {
             if (!user.TryGetProperty("privilege", out var value) || value.ValueKind != System.Text.Json.JsonValueKind.Array)
             {
                 return null;
             }

             return string.Join(',', value.EnumerateArray().Select(element => element.GetString()));
        });
        options.CurrentValue.StateDataFormat = provider.GetRequiredService<PropertiesDataFormatCache>();

        return new WeixinAuthenticationHandler(options, loggerFactory, urlEncoder, systemClock);
    }
}
