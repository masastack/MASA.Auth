// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


using AspNet.Security.OAuth.WorkWeixin;

namespace Masa.Auth.Security.OAuth.Providers.WorkWeixin;

public class WorkWeixinBuilder : ILocalAuthenticationDefaultBuilder, IAuthenticationInject
{
    public string Scheme => WorkWeixinAuthenticationDefaults.AuthenticationScheme;

    public AuthenticationDefaults AuthenticationDefaults => new AuthenticationDefaults
    {
        Scheme = Scheme,
        DisplayName = WorkWeixinAuthenticationDefaults.DisplayName,
        HandlerType = typeof(WorkWeixinAuthenticationHandler),
        Icon = "https://cdn.masastack.com/stack/auth/ico/workweixin.svg",
        CallbackPath = WorkWeixinAuthenticationDefaults.CallbackPath,
        Issuer = WorkWeixinAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = WorkWeixinAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = WorkWeixinAuthenticationDefaults.TokenEndpoint,
        UserInformationEndpoint = WorkWeixinAuthenticationDefaults.UserInformationEndpoint,
        ThirdPartyIdpType = ThirdPartyIdpTypes.WorkWeixin,
        JsonKeyMap = new Dictionary<string, string>
        {
            [UserClaims.Subject] = "userid",
            [UserClaims.Gender] = "gender",
            [UserClaims.Locale] = "address",
            [UserClaims.Picture] = "avatar",
            [UserClaims.Email] = "email"
        }
    };

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddWorkWeixin(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
        });
    }

    public void InjectForHotUpdate(IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WorkWeixinAuthenticationOptions>, OAuthPostConfigureOptions<WorkWeixinAuthenticationOptions, WorkWeixinAuthenticationHandler>>());
    }
}
