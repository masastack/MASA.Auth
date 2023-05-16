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

        var departmentId = Guid.Empty;
        var adminStaffId = Guid.Empty;
        var guestStaffId = Guid.Empty;
        string system = "system", admin = "admin", guest = "guest";

        var userSetter = serviceProvider.GetService<IUserSetter>();
        var auditUser = new IdentityUser() { Id = masaStackConfig.GetDefaultUserId().ToString(), UserName = system };
        var userSetterHandle = userSetter!.Change(auditUser);

        #region Auth

        var userPermission = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "User", "user", "User", "mdi-account-circle", 1, PermissionTypes.Menu);
        var updateAccountPermission = new Permission(MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "AccountUpdate", ElementPermissionCodes.UserAccountUpdate, "", "", 1, PermissionTypes.Element);
        updateAccountPermission.SetParent(userPermission.Id);

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
        var teamAddElement = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetWebId(MasaStackConstant.AUTH), "TeamAdd", "team.add", "", "", 1, PermissionTypes.Element);
        teamAddElement.SetParent(team.Id);
        var teamAddApi = new Permission(Guid.NewGuid(), MasaStackConstant.AUTH, masaStackConfig.GetServiceId(MasaStackConstant.AUTH), "TeamAdd", ElementPermissionCodes.TeamAdd, "", "", 1, PermissionTypes.Api);
        teamAddElement.BindApiPermission(teamAddApi.Id);

        var authMenus = new List<Permission>() {
            userPermission,
            rolePermission,role,permission,team,teamAddElement,teamAddApi,
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"Organization","org","organization/index","mdi-file-tree-outline",4,PermissionTypes.Menu),
            sso,userClaim,identityResource,apiScope,apiResource,client,customLogin,
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"ThirdPartyIdp","thirdPartyIdp","thirdPartyIdp","mdi-arrange-bring-forward",6,PermissionTypes.Menu),
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"Position","position","organization/position","mdi-post",7,PermissionTypes.Menu),
            new Permission(MasaStackConstant.AUTH,masaStackConfig.GetWebId(MasaStackConstant.AUTH),"OperationLog","operationLog","operationLog","mdi-record-circle",8,PermissionTypes.Menu)
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
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"Team","Team","Team","mdi-account-group-outline",2,PermissionTypes.Menu),
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"PublicConfig","PublicConfig","Public","mdi-earth",3,PermissionTypes.Menu),
            new Permission(MasaStackConstant.DCC,masaStackConfig.GetWebId(MasaStackConstant.DCC),"LabelManagement","Label","Label","mdi-label",4,PermissionTypes.Menu),
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
            new Permission(MasaStackConstant.SCHEDULER,masaStackConfig.GetWebId(MasaStackConstant.SCHEDULER),"Team","Team","team","mdi-account-group-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.SCHEDULER,masaStackConfig.GetWebId(MasaStackConstant.SCHEDULER),"ResourceFiles","scheduler.resource","resource","mdi-file-document-outline",2,PermissionTypes.Menu)
        };
        if (!context.Set<Permission>().Any(p => p.SystemId == MasaStackConstant.SCHEDULER))
        {
            await eventBus.PublishAsync(new SeedPermissionsCommand(schedulerMenus));
        }
        #endregion 

        #region Tsc
        var tscMenus = new List<Permission>() {
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Team","Team","team","mdi-account-group-outline",1,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Dashboard","Dashboard","dashboard","mdi-view-dashboard",2,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Log","Log","log","mdi-file-search",3,PermissionTypes.Menu),
            new Permission(MasaStackConstant.TSC,masaStackConfig.GetWebId(MasaStackConstant.TSC),"Trace","Trace","trace","mdi-chart-timeline-variant",4,PermissionTypes.Menu)
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

        if (!context.Set<User>().Any(u => u.Account == admin))
        {
            var addStaffDto = new AddStaffDto
            {
                Name = admin,
                Account = admin,
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
            if (departmentId != Guid.Empty)
            {
                addStaffDto.DepartmentId = departmentId;
            }
            var addStaffCommand = new AddStaffCommand(addStaffDto);
            await eventBus.PublishAsync(addStaffCommand);
            adminStaffId = addStaffCommand!.Result?.Id ?? Guid.Empty;
        }

        if (!context.Set<User>().Any(u => u.Account == system))
        {
            await eventBus.PublishAsync(new AddUserCommand(new AddUserDto
            {
                Id = masaStackConfig.GetDefaultUserId(),
                Account = system,
                Name = system,
                DisplayName = "System",
                Avatar = "https://cdn.masastack.com/stack/images/avatar/mr.zhen.svg",
                Email = "system@masastack.com",
                Enabled = true
            }));
        }

        if (masaStackConfig.IsDemo)
        {
            guestStaffId = await AddDemoGuestDataAsync(context, guest, eventBus, departmentId);
        }

        if (!context.Set<Team>().Any())
        {
            var addTeamDto = new AddTeamDto
            {
                Id = masaStackConfig.GetDefaultTeamId(),
                Type = TeamTypes.Ordinary,
                Name = MasaStackConsts.MASA_STACK_TEAM,
                Avatar = new AvatarValueDto
                {
                    Name = MasaStackConsts.MASA_STACK_TEAM,
                    Color = "blue"
                }
            };
            if (adminStaffId != Guid.Empty)
            {
                addTeamDto.AdminStaffs.Add(adminStaffId);
            }
            if (guestStaffId != Guid.Empty)
            {
                addTeamDto.MemberStaffs.Add(guestStaffId);
            }
            await eventBus.PublishAsync(new AddTeamCommand(addTeamDto));
        }

        #region SSO Client

        var uis = masaStackConfig.GetAllUINames();
        var clientDefaultLogoUrl = "https://cdn.masastack.com/stack/auth/ico/auth-client-default.svg";
        foreach (var ui in uis)
        {
            if (context.Set<Client>().Any(u => u.ClientId == ui.Item1))
            {
                continue;
            }

            await eventBus.PublishAsync(new AddClientCommand(new AddClientDto
            {
                ClientId = ui.Item1,
                ClientName = ui.Item2,
                ClientUri = "",
                RequireConsent = false,
                AllowedScopes = new List<string> { "openid", "profile" },
                RedirectUris = new List<string> { $"{ui.Item3}/signin-oidc" },
                PostLogoutRedirectUris = new List<string> { $"{ui.Item3}/signout-callback-oidc" },
                FrontChannelLogoutUri = $"{ui.Item3}/account/frontchannellogout",
                BackChannelLogoutUri = $"{ui.Item3}/account/backchannellogout",
                LogoUri = clientDefaultLogoUrl
            }));

            //add custom register
            await eventBus.PublishAsync(new AddCustomLoginCommand(new AddCustomLoginDto
            {
                ClientId = ui.Item1,
                Enabled = true,
                Name = ui.Item2,
                Title = ui.Item2,
                RegisterFields = new List<RegisterFieldDto>
                {
                    new RegisterFieldDto(RegisterFieldTypes.PhoneNumber,1,true),
                    new RegisterFieldDto(RegisterFieldTypes.DisplayName,2,false)
                }
            }));
        }

        #endregion

        #region all permission cache
        await eventBus.PublishAsync(new SyncPermissionRedisCommand());
        #endregion

        userSetterHandle.Dispose();
    }

    private async Task<Guid> AddDemoGuestDataAsync(AuthDbContext context, string guest, IEventBus eventBus, Guid departmentId)
    {
        var guestStaffId = Guid.Empty;
        var permissions = await context.Set<Permission>().Select(p => new SubjectPermissionRelationDto(p.Id, true)).ToListAsync();

        if (!context.Set<Role>().Any(r => r.Name == guest))
        {
            var addRoleDto = new AddRoleDto()
            {
                Name = guest,
                Code = guest,
                Description = guest,
                Enabled = true,
                Limit = 0,
                Permissions = permissions
            };

            await eventBus.PublishAsync(new AddRoleCommand(addRoleDto));
        }

        if (!context.Set<User>().Any(u => u.Account == guest))
        {
            var addStaffDto = new AddStaffDto
            {
                Name = guest,
                Account = guest,
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
            if (departmentId != Guid.Empty)
            {
                addStaffDto.DepartmentId = departmentId;
            }

            var addStaffCommand = new AddStaffCommand(addStaffDto);

            await eventBus.PublishAsync(addStaffCommand);

            guestStaffId = addStaffCommand?.Result?.Id ?? Guid.Empty;

            var guestRole = await context.Set<Role>().FirstOrDefaultAsync(p => p.Code == guest);
            var guestUser = await context.Set<User>().FirstOrDefaultAsync(u => u.Account == guest);
            await eventBus.PublishAsync(new UpdateUserAuthorizationCommand(new UpdateUserAuthorizationDto()
            {
                Id = guestUser!.Id,
                Roles = new List<Guid>() { guestRole!.Id }
            }));
        }
        return guestStaffId;
    }
}
