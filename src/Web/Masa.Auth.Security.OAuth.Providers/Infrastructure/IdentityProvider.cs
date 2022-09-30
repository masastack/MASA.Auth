// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class IdentityProvider
{
    readonly static List<IIdentityBuilder> identityBuilders;

    static IdentityProvider()
    {
        var builderTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsInterface is false && type.IsAssignableTo(typeof(IIdentityBuilder)));

        identityBuilders = builderTypes.Select(type => (IIdentityBuilder)type.Assembly.CreateInstance(type.FullName!)!)
                                       .ToList();
    }

    public static Identity GetIdentity(string scheme, ClaimsPrincipal principal)
    {
        var builder = identityBuilders.FirstOrDefault(builder => builder.Scheme == scheme) ?? throw new UserFriendlyException("Get user info failed");
        var identity = builder.BuildIdentity(principal);
        identity.Issuer = scheme;
        return identity;
    }
}
