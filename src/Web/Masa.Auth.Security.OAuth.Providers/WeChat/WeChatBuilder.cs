// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AspNet.Security.OAuth.Weixin.WeixinAuthenticationConstants;

namespace Masa.Auth.Security.OAuth.Providers.WeChat;

public class WeChatBuilder : IIdentityBuilder, ILocalAuthenticationDefaultBuilder, IAuthenticationExternalInject, IAuthenticationSchemeBuilder, IAuthenticationInstanceBuilder
{
    public string Scheme { get; } = WeixinAuthenticationDefaults.AuthenticationScheme;

    public AuthenticationScheme AuthenticationScheme { get; } = new AuthenticationScheme(
        WeixinAuthenticationDefaults.AuthenticationScheme,
        WeixinAuthenticationDefaults.DisplayName,
        typeof(WeixinAuthenticationHandler)
    );

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);
        identity.Subject = principal.FindFirstValue(Claims.UnionId);
        identity.Picture = principal.FindFirstValue(Claims.HeadImgUrl);
        identity.Region = principal.FindFirstValue(Claims.Province);
        identity.Locality = principal.FindFirstValue(Claims.City);

        return identity;
    }

    public AuthenticationDefaults BuildAuthenticationDefaults()
    {
        return new AuthenticationDefaults
        {
            Scheme = "WeChat",
            DisplayName = WeixinAuthenticationDefaults.DisplayName,
            Icon = "https://masa-cdn-dev.oss-cn-hangzhou.aliyuncs.com/wechat.ico",
            CallbackPath = WeixinAuthenticationDefaults.CallbackPath,
            Issuer = WeixinAuthenticationDefaults.Issuer,
            AuthorizationEndpoint = WeixinAuthenticationDefaults.AuthorizationEndpoint,
            TokenEndpoint = WeixinAuthenticationDefaults.TokenEndpoint,
            UserInformationEndpoint = WeixinAuthenticationDefaults.UserInformationEndpoint
        };
    }

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddWeixin(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
        });
    }

    public IAuthenticationHandler CreateInstance(IServiceProvider provider, AuthenticationDefaults authenticationDefaults)
    {
        var (options, loggerFactory, urlEncoder, systemClock) = CreateAuthenticationHandlerInstanceUtilities.BuilderParamter<WeixinAuthenticationOptions>(provider);
        authenticationDefaults.BindOAuthOptions(options.CurrentValue);
        return new WeixinAuthenticationHandler(options, loggerFactory, urlEncoder, systemClock);
    }
}
