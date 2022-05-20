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
            new IdentityResources.Email()
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
                ClientId = "masa.auth.admin.web",
                ClientName = "Masa Auth Web",
                AllowedGrantTypes = GrantTypes.Implicit,
                RedirectUris = { $"https://localhost:18100/signin-oidc" },
                PostLogoutRedirectUris = { $"https://localhost:18100/signout-callback-oidc" },
                AllowedScopes = new [] {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email
                },
                RequireConsent = true,
                //AccessTokenLifetime = 3600, // one hour
                AllowAccessTokensViaBrowser = true // can return access_token to this client
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
