// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthSeedData
{
    public async Task SeedAsync(WebApplicationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        //use event bus publish seed data will cache
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var context = serviceProvider.GetRequiredService<AuthDbContext>();
        var masaStackConfig = serviceProvider.GetRequiredService<IMasaStackConfig>();

        #region Auth
        var rolePermission = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "RolePermission", "RolePermission", "", "mdi-shield-half-full", 2, PermissionTypes.Menu);
        var role = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "Role", "role", "role", "", 1, PermissionTypes.Menu);
        var permission = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "Permission", "permission", "permission/index", "", 2, PermissionTypes.Menu);
        role.SetParent(rolePermission.Id);
        permission.SetParent(rolePermission.Id);

        var sso = new Permission(Guid.NewGuid(), MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "SSO", "sso", "", "mdi-login-variant", 5, PermissionTypes.Menu);
        var userClaim = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "UserClaim", "userClaim", "sso/userClaim", "", 1, PermissionTypes.Menu);
        var identityResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "IdentityResource", "IdentityResource", "sso/identityResource", "", 2, PermissionTypes.Menu);
        var apiScope = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "ApiScope", "ApiScope", "sso/apiScope", "", 3, PermissionTypes.Menu);
        var apiResource = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "ApiResource", "ApiResource", "sso/apiResource", "", 4, PermissionTypes.Menu);
        var client = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "Client", "Client", "sso/client", "", 5, PermissionTypes.Menu);
        var customLogin = new Permission(MasaStackConsts.AUTH_SYSTEM_ID, masaStackConfig.GetUiId("auth"), "CustomLogin", "CustomLogin", "sso/customLogin", "", 6, PermissionTypes.Menu);
        userClaim.SetParent(sso.Id);
        identityResource.SetParent(sso.Id);
        apiScope.SetParent(sso.Id);
        apiResource.SetParent(sso.Id);
        client.SetParent(sso.Id);
        customLogin.SetParent(sso.Id);

        var authMenus = new List<Permission>() {
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"User","user","User","mdi-account-outline",1,PermissionTypes.Menu),
            rolePermission,role,permission,
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"Team","team","team/index","mdi-account-multiple",3,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"Organization","org","organization/index","mdi-file-tree-outline",4,PermissionTypes.Menu),
            sso,userClaim,identityResource,apiScope,apiResource,client,customLogin,
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-home-floor-3",6,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"Position","position","organization/position","mdi-post",7,PermissionTypes.Menu),
            new Permission(MasaStackConsts.AUTH_SYSTEM_ID,masaStackConfig.GetUiId("auth"),"OperationLog","operationLog","operationLog","mdi-record-circle",8,PermissionTypes.Menu),
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.AUTH_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(authMenus));
        }
        #endregion

        #region Pm
        var pmMenus = new List<Permission>() {
            new Permission(MasaStackConsts.PM_SYSTEM_ID,masaStackConfig.GetUiId("pm"),"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.PM_SYSTEM_ID,masaStackConfig.GetUiId("pm"),"Team","Team","Team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.PM_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(pmMenus));
        }
        #endregion

        #region Dcc
        var dccMenus = new List<Permission>() {
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,masaStackConfig.GetUiId("dcc"),"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,masaStackConfig.GetUiId("dcc"),"PublicConfig","PublicConfig","Public","mdi-earth",2,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,masaStackConfig.GetUiId("dcc"),"LabelManagement","Label","Label","mdi-label",3,PermissionTypes.Menu),
            new Permission(MasaStackConsts.DCC_SYSTEM_ID,masaStackConfig.GetUiId("dcc"),"Team","Team","Team","mdi-account-group-outline",4,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.DCC_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(dccMenus));
        }
        #endregion

        #region Mc
        var messageManagement = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "MessageManagement", "messageManagement", "", "fas fa-tasks", 2, PermissionTypes.Menu);
        var sendMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "SendMessage", "sendMessage", "messageTasks/sendMessage", "", 1, PermissionTypes.Menu);
        var messageRecord = new Permission(MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "MessageRecord", "messageRecord", "messageRecords/messageRecordManagement", "", 2, PermissionTypes.Menu);
        sendMessage.SetParent(messageManagement.Id);
        messageRecord.SetParent(messageManagement.Id);

        var messageTemplate = new Permission(Guid.NewGuid(), MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "MessageTemplate", "messageTemplate", "", "mdi-collage", 3, PermissionTypes.Menu);
        var sms = new Permission(MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "Sms", "sms", "messageTemplates/smsTemplateManagement", "", 1, PermissionTypes.Menu);
        var email = new Permission(MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "Email", "email", "messageTemplates/emailTemplateManagement", "", 2, PermissionTypes.Menu);
        var websiteMessage = new Permission(MasaStackConsts.MC_SYSTEM_ID, masaStackConfig.GetUiId("mc"), "WebsiteMessage", "websiteMessage", "messageTemplates/websiteMessageTemplateManagement", "", 3, PermissionTypes.Menu);
        sms.SetParent(messageTemplate.Id);
        email.SetParent(messageTemplate.Id);
        websiteMessage.SetParent(messageTemplate.Id);

        var mcMenus = new List<Permission>() {
            new Permission(MasaStackConsts.MC_SYSTEM_ID,masaStackConfig.GetUiId("mc"),"ChannelManagement","channelManagement","channels/channelManagement","mdi-email-outline",1,PermissionTypes.Menu),
            messageManagement,sendMessage,messageRecord,
            messageTemplate,sms,email,websiteMessage,
            new Permission(MasaStackConsts.MC_SYSTEM_ID,masaStackConfig.GetUiId("mc"),"ReceiverGroup","receiverGroup","receiverGroups/receiverGroupManagement","fas fa-object-ungroup",4,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.MC_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(mcMenus));
        }
        #endregion

        #region Scheduler
        var schedulerMenus = new List<Permission>() {
            new Permission(MasaStackConsts.SCHEDULER_SYSTEM_ID,masaStackConfig.GetUiId("scheduler"),"ResourceFiles","scheduler.resource","pages/resource","mdi-file-document-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.SCHEDULER_SYSTEM_ID,masaStackConfig.GetUiId("scheduler"),"Team","Team","pages/team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.SCHEDULER_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(schedulerMenus));
        }
        #endregion

        #region Tsc
        var tscMenus = new List<Permission>() {
            new Permission(MasaStackConsts.TSC_SYSTEM_ID,masaStackConfig.GetUiId("tsc"),"Team","Team","team","mdi-square",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.TSC_SYSTEM_ID,masaStackConfig.GetUiId("tsc"),"Dashboard","Dashboard","dashboard","mdi-view-dashboard",2,PermissionTypes.Menu),
            new Permission(MasaStackConsts.TSC_SYSTEM_ID,masaStackConfig.GetUiId("tsc"),"Log","Log","dashbord/log","mdi-file-search",3,PermissionTypes.Menu),
            new Permission(MasaStackConsts.TSC_SYSTEM_ID,masaStackConfig.GetUiId("tsc"),"Trace","Trace","dashbord/trace","mdi-chart-timeline-variant",4,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.TSC_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(tscMenus));
        }
        #endregion

        #region Alert
        var alertMenus = new List<Permission>() {
            new Permission(MasaStackConsts.ALERT_SYSTEM_ID,masaStackConfig.GetUiId("alert"),"AlarmRule","AlarmRule","alarmRules","mdi-bell-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConsts.ALERT_SYSTEM_ID,masaStackConfig.GetUiId("alert"),"AlarmHistory","AlarmHistory","alarmHistory","mdi-chart-box",2,PermissionTypes.Menu),
            new Permission(MasaStackConsts.ALERT_SYSTEM_ID,masaStackConfig.GetUiId("alert"),"WebHook","WebHook","webHook","mdi-earth",3,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConsts.ALERT_SYSTEM_ID))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(alertMenus));
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
                PhoneNumber = "15888888888",
                Enabled = true
            }));
        }

        if (masaStackConfig.IsDemo && !context.Set<User>().Any(u => u.Account == "guest"))
        {
            await eventBus.PublishAsync(new AddUserCommand(new AddUserDto
            {
                Name = "guest",
                Account = "guest",
                Password = "guest123",
                DisplayName = "Guest",
                Avatar = "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg",
                Email = "Guest@masastack.com",
                CompanyName = "Masa",
                PhoneNumber = "15666666666",
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

        #region SSO Client

        var uis = masaStackConfig.GetAllUINames();
        foreach (var ui in uis)
        {
            if (context.Set<Client>().Any(u => u.ClientId == ui.Key))
            {
                continue;
            }

            await eventBus.PublishAsync(new AddClientCommand(new AddClientDto
            {
                ClientId = ui.Key,
                ClientName = ui.Key.ToName(),
                ClientUri = "",
                RequireConsent = false,
                AllowedScopes = new List<string> { "openid", "profile" },
                RedirectUris = ui.Value.Select(url => $"{url}/signin-oidc").ToList(),
                PostLogoutRedirectUris = ui.Value.Select(url => $"{url}/signout-callback-oidc").ToList(),
                FrontChannelLogoutUri = $"{ui.Value.First()}/account/frontchannellogout",
                BackChannelLogoutUri = $"{ui.Value.First()}/account/backchannellogout"
            }));
        }

        #endregion
        //await context.SaveChangesAsync();
    }
}
