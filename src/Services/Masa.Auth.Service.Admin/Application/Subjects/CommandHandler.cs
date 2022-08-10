// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly AuthDbContext _authDbContext;
    readonly StaffDomainService _staffDomainService;
    readonly UserDomainService _userDomainService;
    readonly RoleDomainService _roleDomainService;
    readonly IUserContext _userContext;
    readonly IDistributedCacheClient _cache;
    readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;
    readonly ILdapFactory _ldapFactory;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILogger<CommandHandler> _logger;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IConfiguration _configuration;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        IStaffRepository staffRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        AuthDbContext authDbContext,
        StaffDomainService staffDomainService,
        UserDomainService userDomainService,
        RoleDomainService roleDomainService,
        IDistributedCacheClient cache,
        IUserContext userContext,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository,
        ILdapFactory ldapFactory,
        ILdapIdpRepository ldapIdpRepository,
        ILogger<CommandHandler> logger,
        ThirdPartyUserDomainService thirdPartyUserDomainService,
        IMasaConfiguration masaConfiguration)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _authDbContext = authDbContext;
        _staffDomainService = staffDomainService;
        _userDomainService = userDomainService;
        _roleDomainService = roleDomainService;
        _cache = cache;
        _userContext = userContext;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
        _ldapFactory = ldapFactory;
        _ldapIdpRepository = ldapIdpRepository;
        _logger = logger;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
        _configuration = masaConfiguration.Local;
    }

    #region User

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        await VerifyUserRepeatAsync(default, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.IdCard, userDto.Account);

        var user = new User(userDto.Name, userDto.DisplayName ?? "", userDto.Avatar ?? "", userDto.IdCard ?? "", userDto.Account ?? "", userDto.Password, userDto.CompanyName ?? "", userDto.Department ?? "", userDto.Position ?? "", userDto.Enabled, userDto.PhoneNumber ?? "", userDto.Landline, userDto.Email ?? "", userDto.Address, userDto.Gender);
        user.AddRoles(userDto.Roles.ToArray());
        user.AddPermissions(userDto.Permissions);
        await _userRepository.AddAsync(user);
        command.NewUser = user;
        await _userDomainService.SetAsync(user);
        await _roleDomainService.UpdateRoleLimitAsync(userDto.Roles);
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        await VerifyUserRepeatAsync(userDto.Id, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.IdCard, default);

        user.Update(userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
        if (!string.IsNullOrWhiteSpace(userDto.Password))
        {
            //add for update ldap password
            user.UpdatePassword(userDto.Password);
        }
        await _userRepository.UpdateAsync(user);
        await _userDomainService.SetAsync(user);
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.GetDetailAsync(command.User.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        if (user.Account == "admin")
        {
            throw new UserFriendlyException("超级管理员 无法删除");
        }
        if (user.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException("当前用户不能删除自己");
        }

        await _userRepository.RemoveAsync(user);
        await _roleDomainService.UpdateRoleLimitAsync(user.Roles.Select(role => role.RoleId));
        await _userDomainService.RemoveAsync(user.Id);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        var roles = user.Roles.Select(role => role.RoleId).Union(userDto.Roles);
        user.AddRoles(userDto.Roles.ToArray());

        user.AddPermissions(userDto.Permissions);
        await _userRepository.UpdateAsync(user);

        await _roleDomainService.UpdateRoleLimitAsync(roles);
    }

    [EventHandler(1)]
    public async Task UpdateUserPasswordAsync(ResetUserPasswordCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);

        user.UpdatePassword(userDto.Password);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //todo UserDomainService
        var account = validateByAccountCommand.UserAccountValidateDto.Account;
        var password = validateByAccountCommand.UserAccountValidateDto.Password;
        var key = CacheKey.AccountLoginKey(account);
        var loginCache = await _cache.GetAsync<CacheLogin>(key);
        if (loginCache is not null && loginCache.LoginErrorCount >= 5)
        {
            throw new UserFriendlyException("您连续输错密码5次,登录已冻结，请三十分钟后再次尝试");
        }

        var isLdap = validateByAccountCommand.UserAccountValidateDto.IsLdap;
        if (isLdap)
        {
            var ldaps = await _ldapIdpRepository.GetListAsync();
            if (!ldaps.Any())
            {
                throw new UserFriendlyException("没有配置LDAP认证");
            }
            if (ldaps.Count() > 1)
            {
                _logger.LogWarning("存在多个Ldap配置,域账号登录时只使用第一个配置");
            }
            var ldap = ldaps.First();
            var ldapOptions = ldap.Adapt<LdapOptions>();
            var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);

            var ldapUser = await ldapProvider.GetUserByUserNameAsync(account);
            if (ldapUser == null)
            {
                throw new UserFriendlyException("域账号不存在");
            }

            var dc = new Regex("(?<=DC=).+(?=,)").Match(ldapOptions.BaseDn).Value;
            if (!await ldapProvider.AuthenticateAsync($"{dc}\\{account}", password))
            {
                throw new UserFriendlyException("域账号验证失败");
            }

            var thirdPartyUserDtp = new AddThirdPartyUserDto(ldap.Id, true, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser),
                    new AddUserDto
                    {
                        Name = ldapUser.Name,
                        DisplayName = ldapUser.DisplayName,
                        Enabled = true,
                        Email = ldapUser.EmailAddress,
                        Account = ldapUser.SamAccountName,
                        Password = password,
                        Avatar = _configuration.GetValue<string>("Subjects:Avatar")
                    });
            //phone number regular match
            if (Regex.IsMatch(ldapUser.Phone, @"^1[3456789]\d{9}$"))
            {
                thirdPartyUserDtp.User.PhoneNumber = ldapUser.Phone;
            }
            else
            {
                thirdPartyUserDtp.User.Landline = ldapUser.Phone;
            }
            await _thirdPartyUserDomainService.AddThirdPartyUserAsync(thirdPartyUserDtp);
        }

        var user = await _userRepository.FindAsync(u => u.Account == account);
        if (user != null)
        {
            if (!user.Enabled)
            {
                throw new UserFriendlyException("账号已禁用");
            }
            var success = user.VerifyPassword(password);
            if (!success)
            {
                loginCache ??= new() { FreezeTime = DateTimeOffset.Now.AddMinutes(30), Account = account };
                loginCache.LoginErrorCount++;
                var options = new CombinedCacheEntryOptions<CacheLogin>
                {
                    DistributedCacheEntryOptions = new()
                    {
                        AbsoluteExpiration = loginCache.FreezeTime
                    }
                };
                await _cache.SetAsync(key, loginCache, options);
                throw new UserFriendlyException("账号或密码错误");
            }

            if (loginCache is not null)
            {
                await _cache.RemoveAsync<CacheLogin>(key);
            }
            validateByAccountCommand.Result = success;
        }
    }

    [EventHandler]
    public async Task UpdateUserPasswordAsync(UpdateUserPasswordCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        if (!user.VerifyPassword(userModel.OldPassword))
        {
            throw new UserFriendlyException("password verification failed");
        }
        user.UpdatePassword(userModel.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        await VerifyUserRepeatAsync(userModel.Id, userModel.PhoneNumber, default, userModel.Email, default, default);
        user.UpdateBasicInfo(userModel.DisplayName, userModel.PhoneNumber, userModel.Email, userModel.Avatar, userModel.Gender);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.SetAsync(user);
    }

    [EventHandler(1)]
    public async Task UpsertUserAsync(UpsertUserCommand command)
    {
        var userModel = command.User;
        var user = default(User);
        if (userModel.Id != default)
        {
            user = await _userRepository.FindAsync(u => u.Id == userModel.Id);
            if (user is not null)
            {
                await VerifyUserRepeatAsync(userModel.Id, userModel.PhoneNumber, default, userModel.Email, userModel.IdCard, default);
                user.Update(userModel.Name, userModel.DisplayName, userModel.IdCard, userModel.CompanyName, userModel.PhoneNumber, userModel.Email, userModel.Gender);
                await _userRepository.UpdateAsync(user);
                await _userDomainService.SetAsync(user);
                command.NewUser = user.Adapt<UserModel>();
                return;
            }
        }
        user = new User(userModel.Name, userModel.DisplayName ?? "", DefaultUserAttributes.GetDefaultAvatar(userModel.Gender), userModel.IdCard ?? "", userModel.Account ?? "", DefaultUserAttributes.Password, userModel.CompanyName ?? "", "", "", true, userModel.PhoneNumber ?? "", "", userModel.Email ?? "", new(), userModel.Gender);
        await _userRepository.AddAsync(user);
        await _userDomainService.SetAsync(user);
        command.NewUser = user.Adapt<UserModel>(); ;
    }

    [EventHandler(1)]
    public async Task DisableUserAsync(DisableUserCommand command)
    {
        var userModel = command.User;
        var user = await _userRepository.FindAsync(u => u.Account == userModel.Account);
        if (user is null)
            throw new UserFriendlyException($"User with account {userModel.Account} does not exist");

        user.Disabled();
        await _userRepository.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler(1)]
    public async Task VerifyUserRepeatAsync(VerifyUserRepeatCommand command)
    {
        var user = command.User;
        await VerifyUserRepeatAsync(user.Id, user.PhoneNumber, user.Landline, user.Email, user.IdCard, user.Account);
        command.Result = true;
    }

    [EventHandler]
    public async Task SyncUserAutoCompleteAsync(SyncUserAutoCompleteCommand command)
    {
        var users = await _userRepository.GetAllAsync();
        var syncCount = 0;
        while (syncCount < users.Count)
        {
            var syncUsers = users.Skip(syncCount)
                                .Take(command.Dto.OnceExecuteCount)
                                .Adapt<List<UserSelectDto>>();
            await _autoCompleteClient.SetAsync<UserSelectDto, Guid>(syncUsers);
            syncCount += command.Dto.OnceExecuteCount;
        }
    }

    private async Task<User> CheckUserExistAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        if (user is null)
            throw new UserFriendlyException("The current user does not exist");

        return user;
    }

    private async Task VerifyUserRepeatAsync(Guid? userId, string? phoneNumber, string? landline, string? email, string? idCard, string? account)
    {
        Expression<Func<User, bool>> condition = user => false;
        if (!string.IsNullOrEmpty(account))
            condition = condition.Or(user => user.Account == account);
        if (!string.IsNullOrEmpty(phoneNumber))
            condition = condition.Or(user => user.PhoneNumber == phoneNumber);
        if (!string.IsNullOrEmpty(landline))
            condition = condition.Or(user => user.Landline == landline);
        if (!string.IsNullOrEmpty(email))
            condition = condition.Or(user => user.Email == email);
        if (!string.IsNullOrEmpty(idCard))
            condition = condition.Or(user => user.IdCard == idCard);
        if (userId is not null)
        {
            Expression<Func<User, bool>> condition2 = user => user.Id != userId;
            condition = condition2.And(condition);
        }

        var exitUser = await _userRepository.FindAsync(condition);
        if (exitUser is not null)
        {
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == exitUser.PhoneNumber)
                throw new UserFriendlyException($"User with phone number {phoneNumber} already exists");
            if (string.IsNullOrEmpty(landline) is false && landline == exitUser.Landline)
                throw new UserFriendlyException($"User with landline {landline} already exists");
            if (string.IsNullOrEmpty(email) is false && email == exitUser.Email)
                throw new UserFriendlyException($"User with email {email} already exists");
            if (string.IsNullOrEmpty(idCard) is false && idCard == exitUser.IdCard)
                throw new UserFriendlyException($"User with idCard {idCard} already exists");
            if (string.IsNullOrEmpty(account) is false && account == exitUser.Account)
                throw new UserFriendlyException($"User with account {account} already exists");
        }
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddStaffAsync(command.Staff);
    }

    [EventHandler]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        await _staffDomainService.UpdateStaffAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staff = await _authDbContext.Set<Staff>()
                                            .Include(s => s.DepartmentStaffs)
                                            .Include(s => s.TeamStaffs)
                                            .FirstOrDefaultAsync(s => s.Id == command.Staff.Id);
        if (staff == null)
        {
            throw new UserFriendlyException("the current staff not found");
        }
        await _staffRepository.RemoveAsync(staff);

        var teams = staff.TeamStaffs.Select(ts => ts.TeamId);
        var roles = await _authDbContext.Set<TeamRole>()
                                .Where(tr => teams.Contains(tr.TeamId))
                                .Select(tr => tr.RoleId)
                                .ToListAsync();
        await _roleDomainService.UpdateRoleLimitAsync(roles);
    }

    [EventHandler]
    public async Task SyncAsync(SyncStaffCommand command)
    {
        command.Result = await _staffDomainService.SyncStaffAsync(command.Staffs);
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var exist = await _thirdPartyIdpRepository.GetCountAsync(thirdPartyIdp => thirdPartyIdp.Name == thirdPartyIdpDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ThirdPartyIdp with name {thirdPartyIdpDto.Name} already exists");

        var thirdPartyIdp = new ThirdPartyIdp(thirdPartyIdpDto.Name, thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);

        await _thirdPartyIdpRepository.AddAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == thirdPartyIdpDto.Id);
        if (thirdPartyIdp is null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        thirdPartyIdp.Update(thirdPartyIdpDto.DisplayName, thirdPartyIdpDto.Icon, thirdPartyIdpDto.Enabled, thirdPartyIdpDto.IdentificationType, thirdPartyIdpDto.ClientId, thirdPartyIdpDto.ClientSecret, thirdPartyIdpDto.Url, thirdPartyIdpDto.VerifyFile, thirdPartyIdpDto.VerifyType);
        await _thirdPartyIdpRepository.UpdateAsync(thirdPartyIdp);
    }

    [EventHandler]
    public async Task RemoveThirdPartyIdpAsync(RemoveThirdPartyIdpCommand command)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == command.ThirdPartyIdp.Id);
        if (thirdPartyIdp == null)
            throw new UserFriendlyException("The current thirdPartyIdp does not exist");

        await _thirdPartyIdpRepository.RemoveAsync(thirdPartyIdp);
    }

    #endregion

    #region UserSystemData
    [EventHandler(1)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand command)
    {
        var data = command.UserSystemData;
        var item = await _userSystemBusinessDataRepository.FindAsync(userSystemBusinessData => userSystemBusinessData.UserId == data.UserId && userSystemBusinessData.SystemId == data.SystemId);
        if (item is null)
        {
            await _userSystemBusinessDataRepository.AddAsync(new UserSystemBusinessData(data.UserId, data.SystemId, data.Data));
        }
        else
        {
            item.Update(data.Data);
            await _userSystemBusinessDataRepository.UpdateAsync(item);
        }
    }
    #endregion
}
