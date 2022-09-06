// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class AuthenticationDefaultsProvider
{
    readonly static List<IAuthenticationDefaultBuilder> _builders;

    static AuthenticationDefaultsProvider()
    {
        var builderTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.BaseType == typeof(IAuthenticationDefaultBuilder));

        _builders = builderTypes.Select(type => (IAuthenticationDefaultBuilder)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static AuthenticationDefaults GetAuthenticationDefaults(string scheme)
    {
        var builder = _builders.First(builder => builder.Scheme == scheme);
        return builder.BuilderAuthenticationDefaults();
    }

    public static List<AuthenticationDefaults> GetAllAuthenticationDefaults()
    {
        return _builders.Select(builder => builder.BuilderAuthenticationDefaults())
                        .ToList();
    }
}
