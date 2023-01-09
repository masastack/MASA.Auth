// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthSeedData
{
    public async Task SeedAsync(AuthDbContext context, IServiceProvider serviceProvider)
    {
        //use event bus publish seed data will cache
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        #region Auth
        var rolePermission = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "RolePermission", "RolePermission", "", "mdi-shield-half-full", 2, PermissionTypes.Menu);
        var role = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Role", "role", "role", "", 1, PermissionTypes.Menu);
        var permission = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Permission", "permission", "permission/index", "", 2, PermissionTypes.Menu);
        role.SetParent(rolePermission.Id);
        permission.SetParent(rolePermission.Id);

        var sso = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "SSO", "sso", "", "mdi-login-variant", 5, PermissionTypes.Menu);
        var userClaim = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "UserClaim", "userClaim", "sso/userClaim", "", 1, PermissionTypes.Menu);
        var identityResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "IdentityResource", "IdentityResource", "sso/identityResource", "", 2, PermissionTypes.Menu);
        var apiScope = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "ApiScope", "ApiScope", "sso/apiScope", "", 3, PermissionTypes.Menu);
        var apiResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "ApiResource", "ApiResource", "sso/apiResource", "", 4, PermissionTypes.Menu);
        var client = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "Client", "Client", "sso/client", "", 5, PermissionTypes.Menu);
        var customLogin = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID, "CustomLogin", "CustomLogin", "sso/customLogin", "", 6, PermissionTypes.Menu);
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
            await eventBus.PublishAsync(new SeedPermissionsCommand(authMenus));
        }
        #endregion

        #region Pm
        var pmMenus = new List<Permission>() {
            new Permission(MasaStackConsts.PM_SYSTEM_ID,MasaStackConsts.PM_SYSTEM_WEB_APP_ID,"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.PM_SYSTEM_ID,MasaStackConsts.PM_SYSTEM_WEB_APP_ID,"Team","Team","Team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.PM_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(pmMenus));
        }
        #endregion

        #region Dcc
        var dccMenus = new List<Permission>() {
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"PublicConfig","PublicConfig","Public","mdi-earth",2,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"LabelManagement","Label","Label","mdi-label",3,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,MasaStackConsts.DCC_SYSTEM_WEB_APP_ID,"Team","Team","Team","mdi-account-group-outline",4,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.DCC_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(dccMenus));
        }
        #endregion

        #region Mc
        var messageManagement = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageManagement", "messageManagement", "", "fas fa-tasks", 2, PermissionTypes.Menu);
        var sendMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "SendMessage", "sendMessage", "messageTasks/sendMessage", "", 1, PermissionTypes.Menu);
        var messageRecord = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageRecord", "messageRecord", "messageRecords/messageRecordManagement", "", 2, PermissionTypes.Menu);
        sendMessage.SetParent(messageManagement.Id);
        messageRecord.SetParent(messageManagement.Id);

        var messageTemplate = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "MessageTemplate", "messageTemplate", "", "mdi-collage", 3, PermissionTypes.Menu);
        var sms = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "Sms", "sms", "messageTemplates/smsTemplateManagement", "", 1, PermissionTypes.Menu);
        var email = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "Email", "email", "messageTemplates/emailTemplateManagement", "", 2, PermissionTypes.Menu);
        var websiteMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, MasaStackConsts.MC_SYSTEM_WEB_APP_ID, "WebsiteMessage", "websiteMessage", "messageTemplates/websiteMessageTemplateManagement", "", 3, PermissionTypes.Menu);
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
            await eventBus.PublishAsync(new SeedPermissionsCommand(mcMenus));
        }
        #endregion

        #region scheduler
        var schedulerMenus = new List<Permission>() {
            new Permission(MasaStackConsts.SCHEDULER_SYSTEM_ID,MasaStackConsts.SCHEDULER_SYSTEM_WEB_APP_ID,"ResourceFiles","scheduler.resource","pages/resource","mdi-file-document-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.SCHEDULER_SYSTEM_ID,MasaStackConsts.SCHEDULER_SYSTEM_WEB_APP_ID,"Team","Team","pages/team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.SCHEDULER_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(schedulerMenus));
        }
        #endregion

        if (!context.Set<User>().Any(u => u.Account == "admin"))
        {
            await eventBus.PublishAsync(new AddUserCommand(new AddUserDto
            {
                Name = "admin",
                Account = "admin",
                Password = "admin123",
                DisplayName = "Administrator",
                Avatar = "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg",
                Email = "admin@masastack.com",
                CompanyName = "Masa",
                PhoneNumber = "15185856868",
                Enabled = true
            }));
        }

        if (!context.Set<Department>().Any())
        {
            await eventBus.PublishAsync(new UpsertDepartmentCommand(new UpsertDepartmentDto
            {
                Name = MasaStackConsts.ORGANIZATION_NAME,
                Description = MasaStackConsts.ORGANIZATION_DESCRIPTION,
                Enabled = true
            }));
        }

        //await context.SaveChangesAsync();
    }
}
