// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextSeed
{
    public async Task SeedAsync(AuthDbContext context, ILogger<AuthDbContextSeed> logger)
    {
        //todo change to eventbus add(can cache redis)
        #region Auth
        var rolePermission = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "RolePermission", "RolePermission", "", "mdi-shield-half-full", 2, PermissionTypes.Menu);
        var role = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Role", "role", "role", "mdi-circle", 1, PermissionTypes.Menu);
        var permission = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Permission", "permission", "permission/index", "mdi-circle", 2, PermissionTypes.Menu);
        role.SetParent(rolePermission.Id);
        permission.SetParent(rolePermission.Id);

        var sso = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "SSO", "sso", "", "mdi-login-variant", 3, PermissionTypes.Menu);
        var userClaim = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "UserClaim", "userClaim", "sso/userClaim", "mdi-circle", 1, PermissionTypes.Menu);
        var identityResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "IdentityResource", "IdentityResource", "sso/identityResource", "mdi-circle", 2, PermissionTypes.Menu);
        var apiScope = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "ApiScope", "ApiScope", "sso/apiScope", "mdi-circle", 3, PermissionTypes.Menu);
        var apiResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "ApiResource", "ApiResource", "sso/apiResource", "mdi-circle", 4, PermissionTypes.Menu);
        var client = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Client", "Client", "sso/client", "mdi-circle", 5, PermissionTypes.Menu);
        var customLogin = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "CustomLogin", "CustomLogin", "sso/customLogin", "mdi-circle", 6, PermissionTypes.Menu);
        userClaim.SetParent(sso.Id);
        identityResource.SetParent(sso.Id);
        apiScope.SetParent(sso.Id);
        apiResource.SetParent(sso.Id);
        client.SetParent(sso.Id);
        customLogin.SetParent(sso.Id);

        var authMenus = new List<Permission>() {
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"User","user","User","mdi-account-outline",1,PermissionTypes.Menu),
            rolePermission,role,permission,
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Team","team","team/index","mdi-account-multiple",3,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Organization","org","organization/index","mdi-file-tree-outline",4,PermissionTypes.Menu),
            sso,userClaim,identityResource,apiScope,apiResource,client,customLogin,
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-home-floor-3",6,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"Position","position","organization/position","mdi-post",7,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID,"OperationLog","operationLog","operationLog","mdi-record-circle",8,PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.AUTH_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(authMenus);
        }
        #endregion

        #region Pm
        var pmMenus = new List<Permission>() {
            new Permission(MasaStackConsts.PM_SYSTEM_ID,MasaStackConsts.PM_SYSTEM_WEB_APP_ID,"Landscape","Landscape","Landscape","mdi-flag",1,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.PM_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(pmMenus);
        }
        #endregion

        #region Dcc
        var dccMenus = new List<Permission>() {
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Landscape","Landscape","Landscape","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Public","Public","Public","mdi-flag",2,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Label Management","Label","Label","mdi-flag",3,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.DCC_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(dccMenus);
        }
        #endregion

        #region Mc
        var messageManagement = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageManagement", "messageManagement", "", "fas fa-tasks", 2, PermissionTypes.Menu);
        var sendMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "SendMessage", "sendMessage", "messageTasks/sendMessage", "mdi-circle", 1, PermissionTypes.Menu);
        var messageRecord = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageRecord", "messageRecord", "messageRecords/messageRecordManagement", "mdi-circle", 2, PermissionTypes.Menu);
        sendMessage.SetParent(messageManagement.Id);
        messageRecord.SetParent(messageManagement.Id);

        var messageTemplate = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageTemplate", "messageTemplate", "", "mdi-collage", 3, PermissionTypes.Menu);
        var sms = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "Sms", "sms", "messageTemplates/smsTemplateManagement", "mdi-circle", 1, PermissionTypes.Menu);
        var email = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "Email", "email", "messageTemplates/emailTemplateManagement", "mdi-circle", 2, PermissionTypes.Menu);
        var websiteMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "WebsiteMessage", "websiteMessage", "messageTemplates/websiteMessageTemplateManagement", "mdi-circle", 3, PermissionTypes.Menu);
        sms.SetParent(messageTemplate.Id);
        email.SetParent(messageTemplate.Id);
        websiteMessage.SetParent(messageTemplate.Id);

        var mcMenus = new List<Permission>() {
            new Permission(MasaStackConsts.MC_SYSTEM_ID,MasaStackConsts.MC_SYSTEM_WEB_APP_ID,"ChannelManagement","channelManagement","channels/channelManagement","mdi-email-outline",1,PermissionTypes.Menu),
            messageManagement,sendMessage,messageRecord,
            messageTemplate,sms,email,websiteMessage,
            new Permission(MasaStackConsts.MC_SYSTEM_ID,MasaStackConsts.MC_SYSTEM_WEB_APP_ID,"ReceiverGroup","receiverGroup","receiverGroups/receiverGroupManagement","fas fa-object-ungroup",4,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.MC_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(mcMenus);
        }
        #endregion

        #region scheduler
        var schedulerMenus = new List<Permission>() {
            new Permission(MasaStackConsts.SCHEDULER_SYSTEM_ID,MasaStackConsts.SCHEDULER_SYSTEM_WEB_APP_ID,"ResourceFiles","scheduler.resource","pages/resource","mdi-file-document-outline",1,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.SCHEDULER_SYSTEM_ID))
        {
            context.Set<Permission>().AddRange(schedulerMenus);
        }
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
