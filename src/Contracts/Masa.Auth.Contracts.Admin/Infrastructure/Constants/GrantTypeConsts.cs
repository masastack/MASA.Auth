// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;
#warning change Masa.BuildingBlocks.Oidc.Domain
public class GrantTypeConsts
{
    public static ICollection<string> Implicit =>
            new[] { GrantTypes.Implicit };

    public static ICollection<string> ImplicitAndClientCredentials =>
        new[] { GrantTypes.Implicit, GrantTypes.ClientCredentials };

    public static ICollection<string> Code =>
        new[] { GrantTypes.AuthorizationCode };

    public static ICollection<string> CodeAndClientCredentials =>
        new[] { GrantTypes.AuthorizationCode, GrantTypes.ClientCredentials };

    public static ICollection<string> Hybrid =>
        new[] { GrantTypes.Hybrid };

    public static ICollection<string> HybridAndClientCredentials =>
        new[] { GrantTypes.Hybrid, GrantTypes.ClientCredentials };

    public static ICollection<string> ClientCredentials =>
        new[] { GrantTypes.ClientCredentials };

    public static ICollection<string> ResourceOwnerPassword =>
        new[] { GrantTypes.ResourceOwnerPassword };

    public static ICollection<string> ResourceOwnerPasswordAndClientCredentials =>
        new[] { GrantTypes.ResourceOwnerPassword, GrantTypes.ClientCredentials };

    public static ICollection<string> DeviceFlow =>
        new[] { GrantTypes.DeviceFlow };
}
