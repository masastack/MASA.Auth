// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class AuthenticationInstanceProvider
{
    readonly static List<IAuthenticationInstanceBuilder> _builders;

    static AuthenticationInstanceProvider()
    {
        var builderTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsInterface is false && type.IsAssignableTo(typeof(IAuthenticationInstanceBuilder)));

        _builders = builderTypes.Select(type => (IAuthenticationInstanceBuilder)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static IAuthenticationHandler GetInstance(this IServiceProvider provider, AuthenticationDefaults authenticationDefaults)
    {
        var builder = _builders.FirstOrDefault(builder => builder.Scheme == authenticationDefaults.Scheme);
        if (builder is not null)
        {
            return builder.CreateInstance(provider, authenticationDefaults);
        }
        else
        {
            var (options, loggerFactory, urlEncoder, systemClock) = CreateAuthenticationHandlerInstanceUtilities.BuilderParamter<OAuthOptions>(provider);
            return new OAuthHandler<OAuthOptions>(options, loggerFactory, urlEncoder, systemClock);
        }
    }
}
