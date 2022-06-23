// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextSeed
{
    public async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
    {
        var menus = new List<Permission>() {
            new Permission("MASA_Auth","Masa_Auth_Web","User","user","User","mdi-account-outline",PermissionTypes.Menu),
            new Permission("MASA_Auth","Masa_Auth_Web","RolePermission","RolePermission","","mdi-shield-half-full",PermissionTypes.Menu)
        };
    }
}
