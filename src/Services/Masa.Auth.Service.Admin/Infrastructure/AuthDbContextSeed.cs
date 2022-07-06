// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextSeed
{
    public async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
    {
        var menus = new List<Permission>() {
            new Permission("MASA_Auth","Masa_Auth_Web","User","user","User","mdi-account-outline",PermissionTypes.Menu),

            new Permission("MASA_Auth","Masa_Auth_Web","RolePermission","RolePermission","","mdi-shield-half-full",new List<Permission>{
                new Permission("MASA_Auth","Masa_Auth_Web","Role","role","role","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","Permission","permission","permission/index","mdi-circle",PermissionTypes.Menu)
            }),

            new Permission("MASA_Auth","Masa_Auth_Web","Team","team","team/index","mdi-account-multiple",PermissionTypes.Menu),
            new Permission("MASA_Auth","Masa_Auth_Web","Organization","org","organization/index","fa-solid fa-sitemap",PermissionTypes.Menu),

            new Permission("MASA_Auth","Masa_Auth_Web","SSO","sso","","fa-solid fa-id-card",new List<Permission>{
                new Permission("MASA_Auth","Masa_Auth_Web","UserClaim","userClaim","sso/userClaim","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","IdentityResource","IdentityResource","sso/identityResource","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","ApiScope","ApiScope","sso/apiScope","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","ApiResource","ApiResource","sso/apiResource","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","Client","Client","sso/client","mdi-circle",PermissionTypes.Menu),
                new Permission("MASA_Auth","Masa_Auth_Web","CustomLogin","CustomLogin","sso/customLogin","mdi-circle",PermissionTypes.Menu)
            }),

            new Permission("MASA_Auth","Masa_Auth_Web","ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-calendar-check-outline",PermissionTypes.Menu),
            new Permission("MASA_Auth","Masa_Auth_Web","Position","position","organization/position","fa-solid fa-user-plus",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any())
        {
            context.Set<Permission>().AddRange(menus);
        }

        if (!context.Set<User>().Any(u => u.Account == "admin"))
        {
            var adminUser = new User("admin", "Administrator", "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg", "admin", "admin", "Masa");
            context.Set<User>().Add(adminUser);
        }

        if (!context.Set<Department>().Any())
        {
            context.Set<Department>().Add(new Department("MasaStack", "MasaStack Root Department"));
        }

        await context.SaveChangesAsync();
    }
}
