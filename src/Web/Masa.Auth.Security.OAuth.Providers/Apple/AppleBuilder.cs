// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Apple;

public class AppleBuilder : ILocalAuthenticationDefaultBuilder, IAuthenticationInject
{
    public string Scheme { get; } = "Apple";

    public AuthenticationDefaults AuthenticationDefaults { get; } = new AuthenticationDefaults
    {
        HandlerType = typeof(AppleAuthenticationHandler),
        Scheme = "Apple",
        DisplayName = AppleAuthenticationDefaults.DisplayName,
        Icon = "https://cdn.masastack.com/stack/auth/ico/wechat.svg",
        CallbackPath = AppleAuthenticationDefaults.CallbackPath,
        Issuer = AppleAuthenticationDefaults.Issuer,
        AuthorizationEndpoint = AppleAuthenticationDefaults.AuthorizationEndpoint,
        TokenEndpoint = AppleAuthenticationDefaults.TokenEndpoint,
        MapAll = true
    };

    public void Inject(AuthenticationBuilder builder, AuthenticationDefaults authenticationDefault)
    {
        builder.AddApple(authenticationDefault.Scheme, authenticationDefault.DisplayName, options =>
        {
            authenticationDefault.BindOAuthOptions(options);
        });
    }

    public void InjectForHotUpdate(IServiceCollection serviceCollection)
    {        
       serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AppleAuthenticationOptions>, ApplePostConfigureOptions>());
       serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AppleAuthenticationOptions>, OAuthPostConfigureOptions<AppleAuthenticationOptions, AppleAuthenticationHandler>>());
    }
}
