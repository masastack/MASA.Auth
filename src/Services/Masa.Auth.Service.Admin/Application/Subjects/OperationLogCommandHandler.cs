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
        public async Task AddUserOperationLogAsync(AddUserCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddUser, $"添加用户：{command.User.Account}");
        }

        [EventHandler]
        public async Task UpdateUserOperationLogAsync(UpdateUserCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUser, $"编辑用户：{command.User.DisplayName}");
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
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUserAuthorization, $"编辑用户权限：{account}");
        }

        [EventHandler]
        public async Task UpdateUserPasswordOperationLogAsync(ResetUserPasswordCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUserPassword, $"编辑用户密码：{account}");
        }

        [EventHandler]
        public async Task ValidateByAccountOperationLogAsync(ValidateByAccountCommand command)
        {
            var user = await _authDbContext.Set<User>().FirstOrDefaultAsync(user => user.Account == command.UserAccountValidateDto.Account);
            if (user is null) return;
            await _operationLogRepository.AddDefaultAsync(OperationTypes.Login, $"用户：{command.UserAccountValidateDto.Account}登录", user.Id);
        }

        [EventHandler]
        public async Task UpdateUserPasswordOperationLogAsync(UpdateUserPasswordCommand command)
        {
            var account = await GetUserAccountByIdAsync(command.User.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUserPassword, $"编辑用户密码：{account}");
        }

        [EventHandler]
        public async Task UpdateUserBasicInfoOperationLogAsync(UpdateUserBasicInfoCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUser, $"编辑用户：{command.User.DisplayName}");
        }

        #endregion

        #region Staff

        [EventHandler]
        public async Task AddStaffOperationLogAsync(AddStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddStaff, $"添加员工：{command.Staff.DisplayName}");
        }

        [EventHandler]
        public async Task UpdateStaffOperationLogAsync(UpdateStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateStaff, $"编辑员工：{command.Staff.DisplayName}");
        }

        async Task<string> GetStaffDisplayNameByIdAsync(Guid id)
        {
            var displayName = await _authDbContext.Set<Staff>()
                                              .Where(staff => staff.Id == id)
                                              .Select(staff => staff.DisplayName)
                                              .FirstAsync();

            return displayName;
        }

        [EventHandler]
        public async Task RemoveStaffOperationLogAsync(RemoveStaffCommand command)
        {
            var account = await GetStaffDisplayNameByIdAsync(command.Staff.Id);
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateUser, $"编辑用户：{account}");
        }

        [EventHandler]
        public async Task SyncOperationLogAsync(SyncStaffCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.SyncStaff, $"同步员工：{string.Join(',', command.Staffs.Select(staff => staff.DisplayName))}");
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
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddThirdPartyIdp, $"添加第三方平台：{command.ThirdPartyIdp.Name}");
        }

        [EventHandler]
        public async Task UpdateThirdPartyIdpOperationLogAsync(UpdateThirdPartyIdpCommand command)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateThirdPartyIdp, $"编辑第三方平台：{command.ThirdPartyIdp.DisplayName}");
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
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateThirdPartyIdp, $"编辑用户：{LdapConsts.LDAP_NAME}");
        }
        #endregion

        #region Team

        [EventHandler]
        public async Task AddTeamLogAsync(AddTeamCommand addTeamCommand)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddTeam, $"添加团队：{addTeamCommand.AddTeamDto.Name}");
        }

        [EventHandler]
        public async Task UpdateTeamLogAsync(UpdateTeamCommand updateTeamCommand)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.UpdateTeam, $"编辑团队：{updateTeamCommand.UpdateTeamDto.Name}");
        }

        [EventHandler]
        public async Task RemoveTeamLogAsync(RemoveTeamCommand removeTeamCommand)
        {
            if(removeTeamCommand.Result is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveTeam, $"删除团队：{removeTeamCommand.Result.Name}");
        }
        #endregion
    }
}
