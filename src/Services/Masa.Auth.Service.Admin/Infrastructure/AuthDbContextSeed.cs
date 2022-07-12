// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextSeed
{
    public async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
    {
        //todo change to eventbus add(can cache redis)
        var authMenus = new List<Permission>() {
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

        //new Nav("channelManagement", "Permission.ChannelManagement", "mdi-email-outline", "channels/channelManagement", 1),
        //        new Nav("messageManagement", "Permission.MessageManagement", "fas fa-tasks", 1, new List<Nav>
        //        {
        //            new Nav("sendMessage", "Permission.SendMessage", "messageTasks/sendMessage", 2, "messageManagement"),
        //            new Nav("messageRecord", "Permission.MessageRecord", "messageRecords/messageRecordManagement", 2, "messageManagement"),
        //        }),
        //        new Nav("messageTemplateManagement", "Permission.MessageTemplateManagement", "mdi-collage", 1, new List<Nav>
        //        {
        //            new Nav("sms", "Sms", "messageTemplates/smsTemplateManagement", 2, "messageTemplateManagement"),
        //            new Nav("email", "Email", "messageTemplates/emailTemplateManagement", 2, "messageTemplateManagement"),
        //            new Nav("websiteMessage", "WebsiteMessage", "messageTemplates/websiteMessageTemplateManagement", 2, "messageTemplateManagement"),
        //        }),
        //        new Nav("receiverGroupManagement", "Permission.ReceiverGroupManagement", "fas fa-object-ungroup", "receiverGroups/receiverGroupManagement", 1),

        if (!context.Set<Permission>().Any(p => p.SystemId == "MASA_Auth"))
        {
            context.Set<Permission>().AddRange(authMenus);
        }

        var pmMenus = new List<Permission>() {
            new Permission("MASA_Pm","Masa-Pm-Web","Landscape","Landscape.Pm","Landscape","mdi-flag",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == "MASA_Pm"))
        {
            context.Set<Permission>().AddRange(pmMenus);
        }

        var dccMenus = new List<Permission>() {
            new Permission("MASA_Dcc","Masa-Dcc-Web","Landscape","Landscape.Dcc","Landscape","mdi-flag",PermissionTypes.Menu),
            new Permission("MASA_Dcc","Masa-Dcc-Web","Public","Public","Public","mdi-flag",PermissionTypes.Menu),
            new Permission("MASA_Dcc","Masa-Dcc-Web","Label Management","Label","Label","mdi-flag",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == "MASA_Dcc"))
        {
            context.Set<Permission>().AddRange(dccMenus);
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
