// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects
{
    public class OperationLogCommandHandler
    {
        IOperationLogRepository _operationLogRepository;
        AuthDbContext _authDbContext;

        public OperationLogCommandHandler(IOperationLogRepository operationLogRepository, AuthDbContext authDbContext)
        {
            _operationLogRepository = operationLogRepository;
            _authDbContext = authDbContext;
        }

        #region User

        [EventHandler]
        public async Task RegisterUserOperationLogAsync(RegisterUserCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RegisterUser, $"注册用户：{command.Result.Account}", command.Result.Id);
        }

        [EventHandler]
        public async Task AddUserOperationLogAsync(AddUserCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddUser, $"新建用户：{command.Result.Account}");
        }

        [EventHandler]
        public async Task UpdateUserOperationLogAsync(UpdateUserCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUser, $"编辑用户：{command.Result.Account}");
        }

        async Task<string> GetUserAccountByIdAsync(Guid id)
        {
            var account = await _authDbContext.Set<User>()
                                              .Where(user => user.Id == id)
                                              .Select(user => user.Account)
                                              .FirstAsync();

            return account;
        }

        [EventHandler(0)]
        public async Task RemoveUserOperationLogAsync(RemoveUserCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveUser, $"删除用户：{account}");
        }

        [EventHandler]
        public async Task UpdateUserAuthorizationOperationLogAsync(UpdateUserAuthorizationCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUserAuthorization, $"编辑用户权限：{account}");
        }

        [EventHandler]
        public async Task UpdateUserPasswordOperationLogAsync(ResetUserPasswordCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUserPassword, $"编辑用户密码：{account}");
        }

        [EventHandler]
        public async Task ValidateByAccountOperationLogAsync(ValidateByAccountCommand command)
        {
            if (command.Result is not null)
                await _operationLogRepository.AddDefaultAsync(OperationTypes.Login, $"用户登录：使用账号{command.Result.Account}登录", command.Result.Id);
        }

        [EventHandler]
        public async Task UpdateUserPhoneNumberOperationLogAsync(UpdateUserPhoneNumberCommand command)
        {
            if (command.Result is true)
            {
                var account = await GetUserAccountByIdAsync(command.User.Id);
                await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUser, $"编辑用户:将{account}手机号改为{command.User.PhoneNumber}");
            }
        }

        [EventHandler]
        public async Task LoginByPhoneNumberOperationLogAsync(LoginByPhoneNumberCommand command)
        {
            if (command.Result is not null)
            {
                await _operationLogRepository.AddDefaultAsync(OperationTypes.Login, $"用户登录:使用手机号{command.Result.PhoneNumber}登录", command.Result.Id);
            }
        }

        [EventHandler(1)]
        public async Task DisableUserOperationLogAsync(DisableUserCommand command)
        {
            if (command.Result is true)
            {
                await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUser, $"编辑用户:将用户{command.User.Account}禁用");
            }
        }

        [EventHandler]
        public async Task ResetPasswordOperationLogAsync(ResetPasswordCommand command)
        {
            if (command.Result is not null)
            {
                await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUserPassword, $"编辑用户{command.Result.Account}密码");
            }
        }

        [EventHandler]
        public async Task UpdateUserPasswordOperationLogAsync(UpdateUserPasswordCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUserPassword, $"编辑用户密码：{account}");
        }

        [EventHandler]
        public async Task UpdateUserBasicInfoOperationLogAsync(UpdateUserBasicInfoCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditUser, $"编辑用户：{command.User.DisplayName}");
        }

        #endregion

        #region Staff

        [EventHandler]
        public async Task AddStaffOperationLogAsync(AddStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddStaff, $"新建员工：{command.Staff.DisplayName}");
        }

        [EventHandler]
        public async Task UpdateStaffOperationLogAsync(UpdateStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditStaff, $"编辑员工：{command.Staff.DisplayName}");
        }

        [EventHandler]
        public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditStaff, $"编辑员工：{command.Staff.DisplayName}");
        }

        [EventHandler(0)]
        public async Task RemoveStaffOperationLogAsync(RemoveStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveStaff, $"删除员工：{command.Result?.DisplayName}");
        }

        [EventHandler]
        public async Task SyncOperationLogAsync(SyncStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.SyncStaff, $"同步员工：{string.Join(',', command.Staffs.Select(staff => staff.DisplayName))}");
        }

        [EventHandler]
        public async Task UpdateStaffDefaultPasswordAsync(UpdateStaffDefaultPasswordCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditDefaultPassword, $"修改默认密码：将默认密码改为{command.DefaultPassword.DefaultPassword}");
        }

        #endregion

        #region ThirdPartyIdp

        async Task<string> GetIdentityProviderNameByIdAsync(Guid id)
        {
            var name = await _authDbContext.Set<IdentityProvider>()
                                              .Where(i => i.Id == id)
                                              .Select(i => i.Name)
                                              .FirstAsync();

            return name;
        }

        [EventHandler]
        public async Task AddThirdPartyIdpOperationLogAsync(AddThirdPartyIdpCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddThirdPartyIdp, $"新建第三方平台：{command.ThirdPartyIdp.Name}");
        }

        [EventHandler]
        public async Task UpdateThirdPartyIdpOperationLogAsync(UpdateThirdPartyIdpCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditThirdPartyIdp, $"编辑第三方平台：{command.ThirdPartyIdp.DisplayName}");
        }

        [EventHandler(0)]
        public async Task RemoveThirdPartyIdpOperationLogAsync(RemoveThirdPartyIdpCommand command)
        {
            var name = await GetIdentityProviderNameByIdAsync(command.ThirdPartyIdp.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveThirdPartyIdp, $"删除第三方平台：{name}");
        }

        #endregion

        #region Ldap

        [EventHandler]
        public async Task LdapUpsertOperationLogAsync(LdapUpsertCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditThirdPartyIdp, $"编辑用户：{LdapConsts.LDAP_NAME}");
        }
        #endregion

        #region Team

        [EventHandler]
        public async Task AddTeamLogAsync(AddTeamCommand addTeamCommand)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddTeam, $"新建团队：{addTeamCommand.AddTeamDto.Name}");
        }

        [EventHandler]
        public async Task UpdateTeamLogAsync(UpdateTeamCommand updateTeamCommand)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditTeam, $"编辑团队：{updateTeamCommand.UpdateTeamDto.Name}");
        }

        [EventHandler]
        public async Task RemoveTeamLogAsync(RemoveTeamCommand removeTeamCommand)
        {
            if (removeTeamCommand.Result is not null)
                await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveTeam, $"删除团队：{removeTeamCommand.Result.Name}");
        }
        #endregion
    }
}
