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
        Icon = "https://masa-cdn-dev.oss-cn-hangzhou.aliyuncs.com/wechat.ico",
        CallbackPath = WeixinAuthenticationDefaults.CallbackPath,
        Issuer = WeixinAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = WeixinAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = WeixinAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = WeixinAuthenticationDefaults.UserInformationEndpoint
    };

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);
        identity.Subject = principal.FindFirstValue(Claims.UnionId);
        identity.Picture = principal.FindFirstValue(Claims.HeadImgUrl);
        identity.Region = principal.FindFirstValue(Claims.Province);
        identity.Locality = principal.FindFirstValue(Claims.City);

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
        return new WeixinAuthenticationHandler(options, loggerFactory, urlEncoder, systemClock);
    }
}
