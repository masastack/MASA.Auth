// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class LocalAuthenticationDefaultsProvider
{
    readonly static List<ILocalAuthenticationDefaultBuilder> _builders;

    static LocalAuthenticationDefaultsProvider()
    {
        var builderTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsInterface is false && type.IsAssignableTo(typeof(ILocalAuthenticationDefaultBuilder)));

        _builders = builderTypes.Select(type => (ILocalAuthenticationDefaultBuilder)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static AuthenticationDefaults GetAuthenticationDefaults(string scheme)
    {
        var builder = _builders.First(builder => builder.Scheme == scheme);
        return builder.BuildAuthenticationDefaults();
    }

    public static List<AuthenticationDefaults> GetAllAuthenticationDefaults()
    {
        return _builders.Select(builder => builder.BuildAuthenticationDefaults())
                        .ToList();
    }
}
