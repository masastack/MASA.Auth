// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso;

public class InMemoryConfiguration
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new[]
        {
            new ApiResource("clientservice", "CAS Client Service"),
            new ApiResource("productservice", "CAS Product Service"),
            new ApiResource("agentservice", "CAS Agent Service")
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
            new Client
            {
                ClientId = "client.api.service",
                ClientSecrets = new [] { new Secret("clientsecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new [] { "clientservice" }
            },
            new Client
            {
                ClientId = "product.api.service",
                ClientSecrets = new [] { new Secret("productsecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new [] { "clientservice", "productservice" }
            },
            new Client
            {
                ClientId = "agent.api.service",
                ClientSecrets = new [] { new Secret("agentsecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes = new [] { "agentservice", "clientservice", "productservice" }
            }
        };
    }

    public static IEnumerable<TestUser> GetUsers()
    {
        return new[]
        {
            new TestUser
            {
                SubjectId = "10001",
                Username = "edison@hotmail.com",
                Password = "edisonpassword"
            },
            new TestUser
            {
                SubjectId = "10002",
                Username = "andy@hotmail.com",
                Password = "andypassword"
            },
            new TestUser
            {
                SubjectId = "10003",
                Username = "leo@hotmail.com",
                Password = "leopassword"
            }
        };
    }
}
