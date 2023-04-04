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
        var rolePermission = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "RolePermission", "RolePermission", "", "mdi-shield-half-full", 2, PermissionTypes.Menu);
        var role = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "Role", "role", "role", "", 1, PermissionTypes.Menu);
        var permission = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "Permission", "permission", "permission/index", "", 2, PermissionTypes.Menu);
        role.SetParent(rolePermission.Id);
        permission.SetParent(rolePermission.Id);

        var sso = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "SSO", "sso", "", "mdi-card-account-details-outline", 5, PermissionTypes.Menu);
        var userClaim = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "UserClaim", "userClaim", "sso/userClaim", "", 1, PermissionTypes.Menu);
        var identityResource = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "IdentityResource", "IdentityResource", "sso/identityResource", "", 2, PermissionTypes.Menu);
        var apiScope = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "ApiScope", "ApiScope", "sso/apiScope", "", 3, PermissionTypes.Menu);
        var apiResource = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "ApiResource", "ApiResource", "sso/apiResource", "", 4, PermissionTypes.Menu);
        var client = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "Client", "Client", "sso/client", "", 5, PermissionTypes.Menu);
        var customLogin = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "CustomLogin", "CustomLogin", "sso/customLogin", "", 6, PermissionTypes.Menu);
        userClaim.SetParent(sso.Id);
        identityResource.SetParent(sso.Id);
        apiScope.SetParent(sso.Id);
        apiResource.SetParent(sso.Id);
        client.SetParent(sso.Id);
        customLogin.SetParent(sso.Id);

        var team = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "Team", "team", "team/index", "mdi-account-multiple", 3, PermissionTypes.Menu);
        var teadAddElement = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "TeamAdd", "team.add", "", "", 1, PermissionTypes.Element);
        teadAddElement.SetParent(team.Id);

        var authMenus = new List<Permission>() {
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"User","user","User","mdi-account-circle",1,PermissionTypes.Menu),
            rolePermission,role,permission,team,teadAddElement,
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"Organization","org","organization/index","mdi-file-tree-outline",4,PermissionTypes.Menu),
            sso,userClaim,identityResource,apiScope,apiResource,client,customLogin,
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-arrange-bring-forward",6,PermissionTypes.Menu),
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"Position","position","organization/position","mdi-post",7,PermissionTypes.Menu),
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"OperationLog","operationLog","operationLog","mdi-record-circle",8,PermissionTypes.Menu),
            new Permission(MasaStackConstant.AUTH, masaStackConfig.GetServerId(MasaStackConstant.AUTH), "TeamAdd", "api.team.create", "", "", 1, PermissionTypes.Api)
        };

        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.AUTH))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(authMenus));
        }
        #endregion

        #region Pm
        var pmMenus = new List<Permission>() {
            new Permission(MasaStackConstant.PM,masaStackConfig.GetWebId(MasaStackConstant.PM),"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.PM,masaStackConfig.GetWebId(MasaStackConstant.PM),"Team","Team","Team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.PM))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(pmMenus));
        }
        #endregion

        #region Dcc
        var dccMenus = new List<Permission>() {
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"Overview","Overview","Overview","mdi-flag",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"PublicConfig","PublicConfig","Public","mdi-earth",2,PermissionTypes.Menu),
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"LabelManagement","Label","Label","mdi-label",3,PermissionTypes.Menu),
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"Team","Team","Team","mdi-account-group-outline",4,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.DCC))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(dccMenus));
        }
        #endregion

        #region Mc
        var messageManagement = new Permission(Guid.NewGuid(), MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "MessageManagement", "messageManagement", "", "fas fa-tasks", 2, PermissionTypes.Menu);
        var sendMessage = new Permission(MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "SendMessage", "sendMessage", "messageTasks/sendMessage", "", 1, PermissionTypes.Menu);
        var messageRecord = new Permission(MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "MessageRecord", "messageRecord", "messageRecords/messageRecordManagement", "", 2, PermissionTypes.Menu);
        sendMessage.SetParent(messageManagement.Id);
        messageRecord.SetParent(messageManagement.Id);

        var messageTemplate = new Permission(Guid.NewGuid(), MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "MessageTemplate", "messageTemplate", "", "mdi-collage", 3, PermissionTypes.Menu);
        var sms = new Permission(MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "Sms", "sms", "messageTemplates/smsTemplateManagement", "", 1, PermissionTypes.Menu);
        var email = new Permission(MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "Email", "email", "messageTemplates/emailTemplateManagement", "", 2, PermissionTypes.Menu);
        var websiteMessage = new Permission(MasaStackConstant.MC, masaStackConfig.GetWebId(MasaStackConstant.MC), "WebsiteMessage", "websiteMessage", "messageTemplates/websiteMessageTemplateManagement", "", 3, PermissionTypes.Menu);
        sms.SetParent(messageTemplate.Id);
        email.SetParent(messageTemplate.Id);
        websiteMessage.SetParent(messageTemplate.Id);

        var mcMenus = new List<Permission>() {
            new Permission(MasaStackConstant.MC,masaStackConfig.GetWebId(MasaStackConstant.MC),"ChannelManagement","channelManagement","channels/channelManagement","mdi-email-outline",1,PermissionTypes.Menu),
            messageManagement,sendMessage,messageRecord,
            messageTemplate,sms,email,websiteMessage,
            new Permission(MasaStackConstant.MC,masaStackConfig.GetWebId(MasaStackConstant.MC),"ReceiverGroup","receiverGroup","receiverGroups/receiverGroupManagement","fas fa-object-ungroup",4,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.MC))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(mcMenus));
        }
        #endregion

        #region Scheduler
        var schedulerMenus = new List<Permission>() {
            new Permission(MasaStackConstant.SCHEDULER,masaStackConfig.GetWebId(MasaStackConstant.SCHEDULER),"ResourceFiles","scheduler.resource","pages/resource","mdi-file-document-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.SCHEDULER,masaStackConfig.GetWebId(MasaStackConstant.SCHEDULER),"Team","Team","pages/team","mdi-account-group-outline",2,PermissionTypes.Menu),
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.SCHEDULER))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(schedulerMenus));
        }
        #endregion

        #region Tsc
        var tscMenus = new List<Permission>() {
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Team","Team","team","mdi-square",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Dashboard","Dashboard","dashboard","mdi-view-dashboard",2,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Log","Log","dashbord/log","mdi-file-search",3,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Trace","Trace","dashbord/trace","mdi-chart-timeline-variant",4,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.TSC))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(tscMenus));
        }
        #endregion

        #region Alert
        var alertMenus = new List<Permission>() {
            new Permission(MasaStackConstant.ALERT,masaStackConfig.GetWebId(MasaStackConstant.ALERT),"AlarmRule","AlarmRule","alarmRules","mdi-bell-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.ALERT,masaStackConfig.GetWebId(MasaStackConstant.ALERT),"AlarmHistory","AlarmHistory","alarmHistory","mdi-chart-box",2,PermissionTypes.Menu),
            new Permission(MasaStackConstant.ALERT,masaStackConfig.GetWebId(MasaStackConstant.ALERT),"WebHook","WebHook","webHook","mdi-earth",3,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.ALERT))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(alertMenus));
        }
        #endregion

        var teamId = Guid.Empty;
        var departmentId = Guid.Empty;

        if (!context.Set<Department>().Any())
        {
            var dapertmentCommand = new UpsertDepartmentCommand(new UpsertDepartmentDto
            {
                Name = MasaStackConsts.ORGANIZATION_NAME,
                Description = MasaStackConsts.ORGANIZATION_DESCRIPTION,
                Enabled = true
            });
            await eventBus.PublishAsync(dapertmentCommand);
            departmentId = dapertmentCommand.Result;
        }

        if (!context.Set<Team>().Any())
        {
            var addTeamCommand = new AddTeamCommand(new AddTeamDto
            {
                Id = masaStackConfig.GetDefaultTeamId(),
                Type = TeamTypes.Ordinary,
                Name = MasaStackConsts.MASA_STACK_TEAM,
                Avatar = new AvatarValueDto
                {
                    Url = "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg"
                }
            });
            await eventBus.PublishAsync(addTeamCommand);
            teamId = masaStackConfig.GetDefaultTeamId();
        }

        if (!context.Set<User>().Any(u => u.Account == "admin"))
        {
            var addStaffDto = new AddStaffDto
            {
                Name = "admin",
                Account = "admin",
                JobNumber = "9527",
                StaffType = StaffTypes.Internal,
                Gender = GenderTypes.Male,
                Password = masaStackConfig.AdminPwd,
                DisplayName = "Administrator",
                Avatar = "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg",
                Email = "admin@masastack.com",
                CompanyName = "ShuShan",
                PhoneNumber = "15888888888",
                Enabled = true
            };
            if (teamId != Guid.Empty)
            {
                addStaffDto.Teams.Add(teamId);
            }
            if (departmentId != Guid.Empty)
            {
                addStaffDto.DepartmentId = departmentId;
            }

            await eventBus.PublishAsync(new AddStaffCommand(addStaffDto));
        }
        if (masaStackConfig.IsDemo && !context.Set<User>().Any(u => u.Account == "guest"))
        {
            var addStaffDto = new AddStaffDto
            {
                Name = "guest",
                Account = "guest",
                JobNumber = "4399",
                Password = "guest123",
                StaffType = StaffTypes.External,
                Gender = GenderTypes.Female,
                DisplayName = "Guest",
                Avatar = "https://cdn.masastack.com/stack/images/avatar/mr.gu.svg",
                Email = "Guest@masastack.com",
                CompanyName = "ShuShan",
                PhoneNumber = "15666666666",
                Enabled = true,
                IdCard = "330104202002020906"
            };
            if (teamId != Guid.Empty)
            {
                addStaffDto.Teams.Add(teamId);
            }
            if (departmentId != Guid.Empty)
            {
                addStaffDto.DepartmentId = departmentId;
            }

            await eventBus.PublishAsync(new AddStaffCommand(addStaffDto));
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
                FrontChannelLogoutUri = $"{ui.Value.Last()}/account/frontchannellogout",
                BackChannelLogoutUri = $"{ui.Value.Last()}/account/backchannellogout"
            }));
        }

        #endregion
        //await context.SaveChangesAsync();
    }
}
