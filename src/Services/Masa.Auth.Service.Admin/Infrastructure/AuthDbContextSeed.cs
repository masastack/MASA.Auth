// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextSeed
{
    public async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
    {
        //todo change to eventbus add(can cache redis)

        #region Auth
        var authMenus = new List<Permission>() {
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"User","user","User","mdi-account-outline",PermissionTypes.Menu),

            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"RolePermission","RolePermission","","mdi-shield-half-full",new List<Permission>{
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Role","role","role","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Permission","permission","permission/index","mdi-circle",PermissionTypes.Menu)
            }),

            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Team","team","team/index","mdi-account-multiple",PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Organization","org","organization/index","mdi-file-tree-outline",PermissionTypes.Menu),

            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"SSO","sso","","mdi-login-variant",new List<Permission>{
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"UserClaim","userClaim","sso/userClaim","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"IdentityResource","IdentityResource","sso/identityResource","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"ApiScope","ApiScope","sso/apiScope","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"ApiResource","ApiResource","sso/apiResource","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Client","Client","sso/client","mdi-circle",PermissionTypes.Menu),
                new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"CustomLogin","CustomLogin","sso/customLogin","mdi-circle",PermissionTypes.Menu)
            }),

            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-home-floor-3",PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Position","position","organization/position","mdi-post",PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"OperationLog","operationLog","operationLog","mdi-record-circle",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.AUTH_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(authMenus);
        }
        #endregion

        #region Pm
        var pmMenus = new List<Permission>() {
            new Permission(MasaStackConsts.PM_SYSTEM_ID,MasaStackConsts.PM_SYSTEM_WEB_APP_ID,"Landscape","Landscape.Pm","Landscape","mdi-flag",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.PM_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(pmMenus);
        }
        #endregion

        #region Dcc
        var dccMenus = new List<Permission>() {
            new Permission(MasaStackConsts.MC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Landscape","Landscape.Dcc","Landscape","mdi-flag",PermissionTypes.Menu),
            new Permission(MasaStackConsts.MC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Public","Public","Public","mdi-flag",PermissionTypes.Menu),
            new Permission(MasaStackConsts.MC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Label Management","Label","Label","mdi-flag",PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.MC_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(dccMenus);
        }
        #endregion

        #region Mc
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
        #endregion

        if (!context.Set<User>().Any(u => u.Account == "admin"))
        {
            var adminUser = new User("admin", "Administrator", "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg", "admin", "admin", "Masa");
            context.Set<User>().Add(adminUser);
        }

        if (!context.Set<Department>().Any())
        {
            context.Set<Department>().Add(new Department(MasaStackConsts.ORGANIZATION_NAME, MasaStackConsts.ORGANIZATION_DESCRIPTION));
        }

        await context.SaveChangesAsync();
    }
}
