// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class LocalAuthenticationSchemeProvider
{
    readonly static List<IAuthenticationSchemeBuilder> authenticationSchemeBuilders;

    static LocalAuthenticationSchemeProvider()
    {
        var builderTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsInterface is false && type.IsAssignableTo(typeof(IAuthenticationSchemeBuilder)));

        authenticationSchemeBuilders = builderTypes.Select(type => (IAuthenticationSchemeBuilder)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static AuthenticationScheme GetScheme(string scheme)
    {
        var builder = authenticationSchemeBuilders.FirstOrDefault(builder => builder.Scheme == scheme);
        return builder?.AuthenticationScheme ?? new AuthenticationScheme(scheme, scheme, typeof(OAuthHandler<OAuthOptions>));
    }

    public static List<AuthenticationScheme> GetSchemes(IEnumerable<string> schemes)
    {
        return schemes.Select(scheme => GetScheme(scheme)).ToList();
    }
}
