// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly AuthDbContext _authDbContext;
    readonly StaffDomainService _staffDomainService;
    readonly UserDomainService _userDomainService;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IUserContext _userContext;
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;
    readonly ILdapFactory _ldapFactory;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly ILogger<CommandHandler> _logger;
    readonly IEventBus _eventBus;
    readonly Sms _sms;
    readonly IOptions<OidcOptions> _oidcOptions;
    readonly IHttpContextAccessor _httpContextAccessor;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        IStaffRepository staffRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        AuthDbContext authDbContext,
        StaffDomainService staffDomainService,
        UserDomainService userDomainService,
        ThirdPartyUserDomainService thirdPartyUserDomainService,
        IMultilevelCacheClient multilevelCacheClient,
        IDistributedCacheClient distributedCacheClient,
        IUserContext userContext,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository,
        ILdapFactory ldapFactory,
        ILdapIdpRepository ldapIdpRepository,
        ILogger<CommandHandler> logger,
        IEventBus eventBus,
        Sms sms,
        IOptions<OidcOptions> oidcOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _authDbContext = authDbContext;
        _staffDomainService = staffDomainService;
        _userDomainService = userDomainService;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
        _multilevelCacheClient = multilevelCacheClient;
        _distributedCacheClient = distributedCacheClient;
        _userContext = userContext;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
        _ldapFactory = ldapFactory;
        _ldapIdpRepository = ldapIdpRepository;
        _logger = logger;
        _eventBus = eventBus;
        _sms = sms;
        _oidcOptions = oidcOptions;
        _httpContextAccessor = httpContextAccessor;
    }

    #region User

    [EventHandler(1)]
    public async Task RegisterUserAsync(RegisterUserCommand command)
    {
        var model = command.RegisterModel;
        await RegisterVerifyAsync(model);
        var addUserCommand = new AddUserCommand(new AddUserDto()
        {
            Account = model.Account,
            DisplayName = model.DisplayName,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Password = model.Password,
            Avatar = model.Avatar,
            Enabled = true,
        });
        await _eventBus.PublishAsync(addUserCommand);
        command.Result = addUserCommand.Result.Adapt<UserModel>();
    }

    async Task RegisterVerifyAsync(RegisterByEmailModel model)
    {
        if (model.UserRegisterType == UserRegisterTypes.Email)
        {
            var emailCodeKey = CacheKey.EmailCodeRegisterKey(model.Email);
            var emailCode = await _distributedCacheClient.GetAsync<string>(emailCodeKey);
            if (!model.EmailCode.Equals(emailCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_EMAIL_CAPTCHA);
            }
        }
        var smsCodeKey = CacheKey.MsgCodeForRegisterKey(model.PhoneNumber);
        var smsCode = await _distributedCacheClient.GetAsync<string>(smsCodeKey);
        if (!model.SmsCode.Equals(smsCode))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SMS_CAPTCHA);
        }
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        var user = await VerifyUserRepeatAsync(default, userDto.PhoneNumber, userDto.Email, userDto.IdCard, userDto.Account, !command.WhenExisReturn);
        if (user is not null)
        {
            command.Result = user;
            return;
        }
        user = new User(userDto.Id, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Gender);
        user.AddRoles(userDto.Roles);
        user.AddPermissions(userDto.Permissions);
        await AddUserAsync(user);
        command.Result = user;
    }

    async Task AddUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _userDomainService.AddAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        await VerifyUserRepeatAsync(userDto.Id, userDto.PhoneNumber, userDto.Email, userDto.IdCard, userDto.Account);
        user.Update(userDto.Account, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAsync(user);
        command.Result = user;
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        var user = await _userRepository.GetDetailAsync(command.User.Id);
        if (user.IsAdmin())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ADMINISTRATOR_DELETE_ERROR);
        }
        if (user.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CURRENT_USER_DELETE_ERROR);
        }

        await _userRepository.RemoveAsync(user);
        await _userDomainService.RemoveAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        var roles = user.Roles.Select(role => role.RoleId).Union(userDto.Roles);
        user.AddRoles(userDto.Roles);
        user.AddPermissions(userDto.Permissions);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAuthorizationAsync(roles);
    }

    [EventHandler(1)]
    public async Task ResetUserPasswordAsync(ResetUserPasswordCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        user.UpdatePassword(userDto.Password);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task UpdateUserPasswordAsync(UpdateUserPasswordCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        if (string.IsNullOrEmpty(userModel.OldPassword) is false && !user.VerifyPassword(userModel.OldPassword ?? ""))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PASSWORD_FAILED);
        }
        user.UpdatePassword(userModel.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        var userModel = command.User;
        var user = await CheckUserExistAsync(userModel.Id);
        user.UpdateBasicInfo(userModel.DisplayName, userModel.Gender, userModel.CompanyName, userModel.Department, userModel.Position, new AddressValue(userModel.Address.Address, "", "", ""));
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task UpsertUserAsync(UpsertUserCommand command)
    {
        var userModel = command.User;
        var user = default(User);
        var roles = new List<Guid>();
        if (userModel.RoleCodes.Any())
        {
            roles.AddRange(await _authDbContext.Set<Role>()
                                                .Where(role => userModel.RoleCodes.Contains(role.Code))
                                                .Select(role => role.Id)
                                                .ToListAsync());
        }
        if (userModel.Id != default)
        {
            user = await _userRepository.FindAsync(u => u.Id == userModel.Id);
            if (user is not null)
            {
                await VerifyUserRepeatAsync(user.Id, default, default, userModel.IdCard, default);
                user.Update(userModel.Name, userModel.DisplayName!, userModel.IdCard, userModel.CompanyName, userModel.Department, userModel.Gender);
                user.AddRoles(roles);
                await _userRepository.UpdateAsync(user);
                await _userDomainService.UpdateAsync(user);
                command.Result = user.Adapt<UserModel>();
            }
            else
            {
                var addUserDto = userModel.Adapt<AddUserDto>();
                addUserDto.Roles.AddRange(roles);
                var addUserCommand = new AddUserCommand(addUserDto);
                await _eventBus.PublishAsync(addUserCommand);
                command.Result = addUserCommand.Result.Adapt<UserModel>();
            }
        }
        else
        {
            user = await VerifyUserRepeatAsync(default, userModel.PhoneNumber, userModel.Email, userModel.IdCard, userModel.Account, false);
            if (user is not null)
            {
                user.Update(userModel.Name, userModel.DisplayName!, userModel.IdCard, userModel.CompanyName, userModel.Department, userModel.Gender);
                user.AddRoles(roles);
                await _userRepository.UpdateAsync(user);
                await _userDomainService.UpdateAsync(user);
                command.Result = user.Adapt<UserModel>();
            }
            else
            {
                user = new User(userModel.Id, userModel.Name, userModel.DisplayName, default, userModel.IdCard, userModel.Account, userModel.Password, userModel.CompanyName, default, default, true, userModel.PhoneNumber, default, userModel.Email, default, userModel.Gender);
                user.AddRoles(roles);
                await AddUserAsync(user);
                command.Result = user.Adapt<UserModel>();
            }
        }
    }

    [EventHandler]
    public async Task UpdateUserAvatarAsync(UpdateUserAvatarCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        user.UpdateAvatar(userDto.Avatar);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler]
    public async Task VerifyMsgCodeForVerifiyPhoneNumberAsync(VerifyMsgCodeForVerifiyPhoneNumberCommand command)
    {
        var model = command.Model;
        var user = await CheckUserExistAsync(model.UserId);
        var msgCodeKey = CacheKey.MsgCodeForVerifiyUserPhoneNumberKey(model.UserId.ToString(), user.PhoneNumber);
        if (await _sms.VerifyMsgCodeAsync(msgCodeKey, model.Code))
        {
            var resultKey = CacheKey.VerifiyUserPhoneNumberResultKey(user.Id.ToString(), user.PhoneNumber);
            await _distributedCacheClient.SetAsync(resultKey, true, TimeSpan.FromSeconds(60 * 10));
            command.Result = true;
        }
    }

    [EventHandler(1)]
    public async Task UpdateUserPhoneNumberAsync(UpdateUserPhoneNumberCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        var checkCurrentPhoneNumber = string.IsNullOrEmpty(user.PhoneNumber);
        var resultKey = CacheKey.VerifiyUserPhoneNumberResultKey(user.Id.ToString(), user.PhoneNumber);
        if (checkCurrentPhoneNumber is false)
        {
            checkCurrentPhoneNumber = await _distributedCacheClient.GetAsync<bool>(resultKey);
        }
        if (checkCurrentPhoneNumber)
        {
            var key = CacheKey.MsgCodeForUpdateUserPhoneNumberKey(userDto.Id.ToString(), userDto.PhoneNumber);
            if (await _sms.VerifyMsgCodeAsync(key, userDto.VerificationCode))
            {
                await VerifyUserRepeatAsync(user.Id, userDto.PhoneNumber, default, default, default);
                user.UpdatePhoneNumber(userDto.PhoneNumber);
                await _userRepository.UpdateAsync(user);
                await _userDomainService.UpdateAsync(user);
                await _distributedCacheClient.RemoveAsync<bool>(resultKey);
                command.Result = true;
            }
        }
    }

    [EventHandler(1)]
    public async Task LoginByPhoneNumberAsync(LoginByPhoneNumberCommand command)
    {
        var model = command.Model;
        var user = await _userRepository.FindWithIncludAsync(u => u.PhoneNumber == model.PhoneNumber, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}"
        });
        if (user is null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_NOT_EXIST, model.PhoneNumber);
        }
        var key = "";
        if (model.RegisterLogin)
        {
            key = CacheKey.MsgCodeForRegisterKey(model.PhoneNumber);
        }
        else
        {
            key = CacheKey.MsgCodeForLoginKey(user.Id.ToString(), model.PhoneNumber);
        }
        if (!await _sms.VerifyMsgCodeAsync(key, model.Code))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_CAPTCHA);
        }
        command.Result = await UserSplicingDataAsync(user);
    }

    [EventHandler]
    public async Task RemoveUserRolesAsync(RemoveUserRolesCommand command)
    {
        var userModel = command.User;
        var user = await _authDbContext.Set<User>()
                                 .Include(u => u.Roles)
                                 .FirstAsync(u => u.Id == userModel.Id);
        var roleIds = await _authDbContext.Set<Role>()
                                    .Where(role => userModel.RoleCodes.Contains(role.Code))
                                    .Select(role => role.Id)
                                    .ToListAsync();
        user.RemoveRoles(roleIds);
        await _userRepository.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task DisableUserAsync(DisableUserCommand command)
    {
        var userModel = command.User;
        var user = await _userRepository.FindAsync(u => u.Account == userModel.Account);
        if (user is null)
            throw new UserFriendlyException(UserFriendlyExceptionCodes.ACCOUNT_NOT_EXIST, userModel.Account);

        user.Disabled();
        await _userRepository.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler(1)]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //todo UserDomainService
        var account = validateByAccountCommand.UserAccountValidateDto.Account;
        var password = validateByAccountCommand.UserAccountValidateDto.Password;
        var key = CacheKey.AccountLoginKey(account);
        var loginCache = await _distributedCacheClient.GetAsync<CacheLogin>(key);
        if (loginCache is not null && loginCache.LoginErrorCount >= 5)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LOGIN_FREEZE);
        }

        var isLdap = validateByAccountCommand.UserAccountValidateDto.IsLdap;
        if (isLdap)
        {
            var ldaps = await _ldapIdpRepository.GetListAsync();
            if (!ldaps.Any())
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LDAP_NOT_CONFIGURED);
            }
            if (ldaps.Count() > 1)
            {
                _logger.LogWarning("There are multiple Ldap configurations, and the first configuration is used when logging in with a domain account");
            }
            var ldap = ldaps.First();
            var ldapOptions = ldap.Adapt<LdapOptions>();
            var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);

            var ldapUser = await ldapProvider.GetUserByUserNameAsync(account);
            if (ldapUser == null)
            {
                throw new UserFriendlyException(UserFriendlyExceptionCodes.LDAP_ACCOUNT_NOTEXIST, account);
            }

            var dc = new Regex("(?<=DC=).+(?=,)").Match(ldapOptions.BaseDn).Value;
            if (!await ldapProvider.AuthenticateAsync($"{dc}\\{account}", password))
            {
                throw new UserFriendlyException(UserFriendlyExceptionCodes.LDAP_ACCOUNT_VALIDATION_FAILED, account);
            }

            var upsertThirdPartyUserCommand = new UpsertLdapUserCommand(ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser), ldapUser.Name, ldapUser.DisplayName, ldapUser.Phone, ldapUser.EmailAddress, ldapUser.SamAccountName, ldapUser.Phone);
            await _eventBus.PublishAsync(upsertThirdPartyUserCommand);
            //get real user account
            account = upsertThirdPartyUserCommand.Result.Account;
        }

        var user = await _userRepository.FindWithIncludAsync(u => u.Account == account || u.PhoneNumber == account, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}"
        });

        if (user == null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.ACCOUNT_NOT_EXIST, account);
        }

        if (!user.Enabled)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ACCOUNT_DISABLED);
        }

        if (!isLdap)
        {
            if (!user.VerifyPassword(password))
            {
                loginCache ??= new() { FreezeTime = DateTimeOffset.Now.AddMinutes(30), Account = account };
                loginCache.LoginErrorCount++;
                await _distributedCacheClient.SetAsync(key, loginCache, loginCache.FreezeTime);
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PASSWORD_FAILED);
            }

            if (loginCache is not null)
            {
                await _distributedCacheClient.RemoveAsync<CacheLogin>(key);
            }
        }

        validateByAccountCommand.Result = await UserSplicingDataAsync(user);
    }

    async Task<UserDetailDto> UserSplicingDataAsync(User user)
    {
        UserDetailDto userDetailDto = user;
        var staff = await _multilevelCacheClient.GetAsync<CacheStaff>(CacheKey.StaffKey(user.Id));
        userDetailDto.StaffId = staff?.Id;
        userDetailDto.StaffDisplayName = staff?.DisplayName;
        userDetailDto.CurrentTeamId = staff?.CurrentTeamId;
        return userDetailDto;
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
                                .Select(user => new UserSelectDto(user.Id, user.Name, user.DisplayName, user.Account, user.PhoneNumber, user.Email, user.Avatar));
            await _autoCompleteClient.SetBySpecifyDocumentAsync(syncUsers);
            syncCount += command.Dto.OnceExecuteCount;
        }
    }

    private async Task<User> CheckUserExistAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        if (user is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);

        return user;
    }

    private async Task<User?> VerifyUserRepeatAsync(Guid? userId, string? phoneNumber, string? email, string? idCard, string? account, bool throwException = true)
    {
        Expression<Func<User, bool>> condition = user => false;
        condition = condition.Or(!string.IsNullOrEmpty(account), user => user.Account == account);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), user => user.PhoneNumber == phoneNumber || user.Account == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), user => user.Email == email);
        condition = condition.Or(!string.IsNullOrEmpty(idCard), user => user.IdCard == idCard);
        condition = condition.And(userId is not null, user => user.Id != userId);

        var existUser = await _authDbContext.Set<User>()
                                           .Include(u => u.Roles)
                                           .FirstOrDefaultAsync(condition);
        if (existUser is not null)
        {
            if (string.IsNullOrEmpty(phoneNumber) is false && string.IsNullOrEmpty(existUser.PhoneNumber) is false && phoneNumber != existUser.PhoneNumber)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.PHONE_NUMBER_MISMATCH, existUser.PhoneNumber, phoneNumber);
            if (account != existUser.Account && phoneNumber != existUser.PhoneNumber && phoneNumber == existUser.Account)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_PHONE_NUMBER_EXIST, phoneNumber);
            if (throwException is false) return existUser;
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existUser.PhoneNumber)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && email == existUser.Email)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && idCard == existUser.IdCard)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(account) is false && account == existUser.Account)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_ACCOUNT_EXIST, account);
        }
        return existUser;
    }

    [EventHandler(1)]
    public async Task ResetPasswordAsync(ResetPasswordCommand command)
    {
        var resetType = command.ResetPasswordType;
        string? key;
        switch (resetType)
        {
            case ResetPasswordTypes.PhoneNumber:
                key = CacheKey.MsgCodeForgotPasswordKey(command.Voucher);
                break;
            case ResetPasswordTypes.Email:
                key = CacheKey.EmailCodeForgotPasswordKey(command.Voucher);
                break;
            default:
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_RESET_PASSWORD_TYPE);
        }
        var captcha = await _distributedCacheClient.GetAsync<string>(key);
        if (!command.Captcha.Equals(captcha))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_CAPTCHA);
        }
        //reset password
        var user = await _userRepository.GetByVoucherAsync(command.Voucher);
        user.UpdatePassword(command.Password);
        await _userRepository.UpdateAsync(user);
        command.Result = user;
    }

    [EventHandler]
    public async Task LoginByAccountAsync(LoginByAccountCommand command)
    {
        var httpClient = new HttpClient();
#if DEBUG
        var docUrl = "http://localhost:18200";
#else
        var docUrl = _oidcOptions.Value.Authority;
#endif
        var disco = await httpClient.GetDiscoveryDocumentAsync(docUrl);
        var loginResult = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _oidcOptions.Value.ClientId,
            Scope = "openid profile offline_access",
            UserName = command.Account,
            Password = command.Password
        });
        if (loginResult.IsError)
            throw new UserFriendlyException(loginResult.Error);

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(BusinessConsts.SWAGGER_TOKEN, loginResult.AccessToken, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7)
        });
    }

    #endregion

    #region Staff

    [EventHandler(1)]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await VerifyStaffRepeatAsync(default, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, !command.WhenExisReturn);
        if (staff is not null) return;

        command.Result = await AddStaffAsync(staffDto);
    }

    async Task<Staff> AddStaffAsync(AddStaffDto staffDto)
    {
        var addUserDto = new AddUserDto(default, staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName, staffDto.Enabled, staffDto.PhoneNumber, default, staffDto.Email, staffDto.Address, default, staffDto.Position, default, staffDto.Password, staffDto.Gender, default, default);
        var addStaffBeforeEvent = new AddStaffBeforeDomainEvent(addUserDto, staffDto.Position);
        await _staffDomainService.AddBeforeAsync(addStaffBeforeEvent);
        var staff = new Staff(
                addStaffBeforeEvent.UserId,
                staffDto.Name,
                staffDto.DisplayName,
                staffDto.Avatar,
                staffDto.IdCard,
                staffDto.CompanyName,
                staffDto.Gender,
                staffDto.PhoneNumber,
                staffDto.Email,
                staffDto.JobNumber,
                addStaffBeforeEvent.PositionId,
                staffDto.StaffType,
                staffDto.Enabled,
                staffDto.Address
            );
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.AddAsync(staff);
        await _staffDomainService.AddAfterAsync(new(staff));
        return staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await CheckStaffExistAsync(staffDto.Id);
        await VerifyStaffRepeatAsync(staffDto.Id, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard);
        await UpdateStaffAsync(staff, staffDto);
        command.Result = staff;
    }

    async Task UpdateStaffAsync(Staff staff, UpdateStaffDto staffDto)
    {
        var updateStaffEvent = new UpdateStaffBeforeDomainEvent(staffDto.Position);
        await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);

        staff.Update(
            updateStaffEvent.PositionId, staffDto.StaffType, staffDto.Enabled, staffDto.Name,
            staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName,
            staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Gender);
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        var teams = staff.TeamStaffs.Select(team => team.TeamId).Union(staffDto.Teams).Distinct().ToList();
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);
        await _staffDomainService.UpdateAfterAsync(new(staff, teams));
    }

    [EventHandler(1)]
    public async Task ChangeStaffCurrentTeamAsync(UpdateStaffCurrentTeamCommand updateStaffCurrentTeamCommand)
    {
        var staff = await _staffRepository.FindAsync(s => s.UserId == updateStaffCurrentTeamCommand.UserId);
        if (staff == null)
        {
            _logger.LogError($"Can`t find staff by UserId = {updateStaffCurrentTeamCommand.UserId}");
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);
        }
        staff.SetCurrentTeam(updateStaffCurrentTeamCommand.TeamId);
        await _staffRepository.UpdateAsync(staff);
        updateStaffCurrentTeamCommand.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
    {
        var staffModel = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == command.Staff.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateBasicInfo(staffModel.DisplayName, staffModel.Gender, staffModel.PhoneNumber, staffModel.Email);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpsertStaffAsync(UpsertStaffCommand command)
    {
        var staffDto = command.Staff;
        var staff = await VerifyStaffRepeatAsync(default, staffDto.JobNumber, staffDto.PhoneNumber, staffDto.Email, staffDto.IdCard, false);
        if (staff is not null)
        {
            var updateStaffDto = staffDto.Adapt<UpdateStaffDto>();
            updateStaffDto.Id = staff.Id;
            await UpdateStaffAsync(staff, updateStaffDto);
            command.Result = staff;
        }
        else
        {
            command.Result = await AddStaffAsync(staffDto);
        }
    }

    [EventHandler(1)]
    public async Task UpsertStaffForLdapAsync(UpsertStaffForLdapCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == staffDto.UserId);
        if (staff is not null)
        {
            var updateStaffEvent = new UpdateStaffBeforeDomainEvent(default);
            await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);
            staff.UpdateForLdap(staffDto.Name, staffDto.DisplayName, staffDto.PhoneNumber, staffDto.Email);
            await _staffRepository.UpdateAsync(staff);
            await _staffDomainService.UpdateAfterAsync(new(staff, default));
            command.Result = staff;
        }
        else
        {
            var addStaffDto = new AddStaffDto
            {
                Name = staffDto.Name,
                DisplayName = staffDto.DisplayName,
                Enabled = true,
                Email = staffDto.Email,
                PhoneNumber = staffDto.PhoneNumber,
                JobNumber = staffDto.JobNumber,
                StaffType = StaffTypes.Internal
            };
            await VerifyStaffRepeatAsync(default, addStaffDto.JobNumber, addStaffDto.PhoneNumber, addStaffDto.Email, addStaffDto.IdCard);
            command.Result = await AddStaffAsync(addStaffDto);
        }
    }

    [EventHandler(1)]
    public async Task UpdateStaffAvatarAsync(UpdateStaffAvatarCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == staffDto.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateAvatar(staffDto.Avatar);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staff = await CheckStaffExistAsync(command.Staff.Id);
        await _staffRepository.RemoveAsync(staff);
        await _staffDomainService.RemoveAsync(new(staff));
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task SyncStaffAsync(SyncStaffCommand command)
    {
        var syncResults = new SyncStaffResultsDto();
        command.Result = syncResults;
        var syncStaffs = command.Staffs;
        //validation
        var validator = new SyncStaffValidator();
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var staff = syncStaffs[i];
            var result = validator.Validate(staff);
            if (result.IsValid is false)
            {
                syncResults[i] = new()
                {
                    JobNumber = staff.JobNumber,
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        CheckDuplicate(Staff => Staff.PhoneNumber);
        CheckDuplicate(Staff => Staff.JobNumber);
        CheckDuplicate(Staff => Staff.Email);
        CheckDuplicate(Staff => Staff.IdCard);
        if (syncResults.IsValid) return;

        var defaultPasswordQuery = new StaffDefaultPasswordQuery();
        await _eventBus.PublishAsync(defaultPasswordQuery);
        string defaultPassword = defaultPasswordQuery.Result.DefaultPassword.WhenNullOrEmptyReplace(DefaultUserAttributes.Password);
        //sync user
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var syncStaff = syncStaffs[i];
            try
            {
                var existStaff = await VerifyStaffRepeatAsync(default, syncStaff.JobNumber, syncStaff.PhoneNumber, syncStaff.Email, syncStaff.IdCard, false);
                if (existStaff is not null)
                {
                    if (existStaff.JobNumber != syncStaff.JobNumber.WhenNullOrEmptyReplace(existStaff.JobNumber))
                    {
                        syncResults[i] = new()
                        {
                            JobNumber = syncStaff.JobNumber,
                            Errors = new()
                            {
                                $"The employee whose mobile phone number is {syncStaff.PhoneNumber} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"
                            }
                        };
                    }
                    else if (existStaff.PhoneNumber != syncStaff.PhoneNumber.WhenNullOrEmptyReplace(existStaff.PhoneNumber))
                    {
                        syncResults[i] = new()
                        {
                            JobNumber = syncStaff.JobNumber,
                            Errors = new()
                            {
                                $"The employee whose job number is {syncStaff.JobNumber}, the corresponding mobile phone number is {existStaff.PhoneNumber}, which does not match the mobile phone number {syncStaff.PhoneNumber}"
                            }
                        };
                    }
                    else
                    {
                        var updateStaffEvent = new UpdateStaffBeforeDomainEvent(syncStaff.Position);
                        await _staffDomainService.UpdateBeforeAsync(updateStaffEvent);
                        existStaff.UpdateBasicInfo(syncStaff.Name, syncStaff.DisplayName, syncStaff.Email, syncStaff.IdCard, syncStaff.Gender, updateStaffEvent.PositionId, syncStaff.StaffType);
                        await _staffRepository.UpdateAsync(existStaff);
                        await _staffDomainService.UpdateAfterAsync(new(existStaff, default));
                    }
                }
                else
                {
                    var addStaffDto = new AddStaffDto
                    {
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Enabled = true,
                        Email = syncStaff.Email,
                        Password = defaultPassword,
                        PhoneNumber = syncStaff.PhoneNumber,
                        JobNumber = syncStaff.JobNumber,
                        IdCard = syncStaff.IdCard,
                        Position = syncStaff.Position,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                    };
                    await AddStaffAsync(addStaffDto);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex is UserFriendlyException ? ex.Message : "Unknown exception, please contact the administrator";
                syncResults[i] = new()
                {
                    JobNumber = syncStaff.JobNumber,
                    Errors = new() { errorMsg }
                };
            }
        }

        void CheckDuplicate(Expression<Func<SyncStaffDto, string?>> selector)
        {
            var func = selector.Compile();
            if (syncStaffs.Where(staff => func(staff) is not null).IsDuplicate(func, out List<SyncStaffDto>? duplicate))
            {
                for (var i = 0; i < syncStaffs.Count; i++)
                {
                    var staff = syncStaffs[i];
                    syncResults[i] = new()
                    {
                        JobNumber = staff.JobNumber,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }

        string Convert(string? str)
        {
            return string.IsNullOrEmpty(str) ? "empty" : str;
        }
    }

    private async Task<Staff> CheckStaffExistAsync(Guid staffId)
    {
        var staff = await _staffRepository.GetDetailByIdAsync(staffId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_FOUND);

        return staff;
    }

    private async Task<Staff?> VerifyStaffRepeatAsync(Guid? staffId, string? jobNumber, string? phoneNumber, string? email, string? idCard, bool throwException = true)
    {
        Expression<Func<Staff, bool>> condition = staff => false;
        condition = condition.Or(!string.IsNullOrEmpty(jobNumber), staff => staff.JobNumber == jobNumber);
        condition = condition.Or(!string.IsNullOrEmpty(phoneNumber), staff => staff.PhoneNumber == phoneNumber);
        condition = condition.Or(!string.IsNullOrEmpty(email), staff => staff.Email == email);
        condition = condition.Or(!string.IsNullOrEmpty(idCard), staff => staff.IdCard == idCard);
        condition = condition.And(staffId is not null, staff => staff.Id != staffId);

        var existStaff = await _staffRepository.FindAsync(condition);
        if (existStaff is not null)
        {
            if (throwException is false) return existStaff;
            if (string.IsNullOrEmpty(phoneNumber) is false && phoneNumber == existStaff.PhoneNumber)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_PHONE_NUMBER_EXIST, phoneNumber);
            if (string.IsNullOrEmpty(email) is false && email == existStaff.Email)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_EMAIL_EXIST, email);
            if (string.IsNullOrEmpty(idCard) is false && idCard == existStaff.IdCard)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_ID_CARD_EXIST, idCard);
            if (string.IsNullOrEmpty(jobNumber) is false && jobNumber == existStaff.JobNumber)
                throw new UserFriendlyException(UserFriendlyExceptionCodes.STAFF_JOB_NUMBER_EXIST, jobNumber);
        }
        return existStaff;
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler(1)]
    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var exist = await _thirdPartyIdpRepository.GetCountAsync(thirdPartyIdp => thirdPartyIdp.Name == thirdPartyIdpDto.Name) > 0;
        if (exist)
            throw new UserFriendlyException(UserFriendlyExceptionCodes.THIRD_PARTY_IDP_NAME_EXIST, thirdPartyIdpDto.Name);

        var thirdPartyIdp = new ThirdPartyIdp(
            thirdPartyIdpDto.Name,
            thirdPartyIdpDto.DisplayName,
            thirdPartyIdpDto.Icon,
            thirdPartyIdpDto.Enabled,
            thirdPartyIdpDto.ThirdPartyIdpType,
            thirdPartyIdpDto.ClientId,
            thirdPartyIdpDto.ClientSecret,
            thirdPartyIdpDto.CallbackPath,
            thirdPartyIdpDto.AuthorizationEndpoint,
            thirdPartyIdpDto.TokenEndpoint,
            thirdPartyIdpDto.UserInformationEndpoint,
            thirdPartyIdpDto.AuthenticationType,
            thirdPartyIdpDto.MapAll,
            JsonSerializer.Serialize(thirdPartyIdpDto.JsonKeyMap));

        await _thirdPartyIdpRepository.AddAsync(thirdPartyIdp);
    }

    [EventHandler(1)]
    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpCommand command)
    {
        var thirdPartyIdpDto = command.ThirdPartyIdp;
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == thirdPartyIdpDto.Id);
        if (thirdPartyIdp is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRD_PARTY_IDP_NOT_EXIST);

        thirdPartyIdp.Update(
            thirdPartyIdpDto.DisplayName,
            thirdPartyIdpDto.Icon,
            thirdPartyIdpDto.Enabled,
            thirdPartyIdpDto.ClientId,
            thirdPartyIdpDto.ClientSecret,
            thirdPartyIdpDto.CallbackPath,
            thirdPartyIdpDto.AuthorizationEndpoint,
            thirdPartyIdpDto.TokenEndpoint,
            thirdPartyIdpDto.UserInformationEndpoint,
            thirdPartyIdpDto.MapAll,
            thirdPartyIdpDto.JsonKeyMap);
        await _thirdPartyIdpRepository.UpdateAsync(thirdPartyIdp);
    }

    [EventHandler(1)]
    public async Task RemoveThirdPartyIdpAsync(RemoveThirdPartyIdpCommand command)
    {
        var thirdPartyIdp = await _authDbContext.Set<ThirdPartyIdp>()
                                                .FirstOrDefaultAsync(tpIdp => tpIdp.Id == command.ThirdPartyIdp.Id);
        if (thirdPartyIdp == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRD_PARTY_IDP_NOT_EXIST);

        await _thirdPartyIdpRepository.RemoveAsync(thirdPartyIdp);
        var removeThirdUserComman = new RemoveThirdPartyUserCommand(thirdPartyIdp.Id);
        await _eventBus.PublishAsync(removeThirdUserComman);
    }

    #endregion

    #region ThirdPartyUser

    [EventHandler]
    public async Task RegisterThirdPartyUserAsync(RegisterThirdPartyUserCommand command)
    {
        var model = command.Model;
        await BindVerifyAsync(model);
        var addThirdPartyUserExternalCommand = new AddThirdPartyUserExternalCommand(new AddThirdPartyUserModel
        {
            ThridPartyIdentity = model.ThridPartyIdentity,
            ExtendedData = model.ExtendedData,
            ThirdPartyIdpType = model.ThirdPartyIdpType,
            User = new AddUserModel
            {
                Account = model.Account,
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Avatar = model.Avatar,
                Password = model.Password
            }
        }, true);
        await _eventBus.PublishAsync(addThirdPartyUserExternalCommand);
        command.Result = addThirdPartyUserExternalCommand.Result;
    }

    async Task BindVerifyAsync(RegisterByEmailModel model)
    {
        if (model.UserRegisterType == UserRegisterTypes.Email)
        {
            var emailCodeKey = CacheKey.EmailCodeBindKey(model.Email);
            var emailCode = await _distributedCacheClient.GetAsync<string>(emailCodeKey);
            if (!model.EmailCode.Equals(emailCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_EMAIL_CAPTCHA);
            }
        }
        var smsCodeKey = CacheKey.MsgCodeForBindKey(model.PhoneNumber);
        var smsCode = await _distributedCacheClient.GetAsync<string>(smsCodeKey);
        if (!model.SmsCode.Equals(smsCode))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SMS_CAPTCHA);
        }
    }

    [EventHandler(1)]
    public async Task AddThirdPartyUserAsync(AddThirdPartyUserCommand command)
    {
        var thirdPartyUserDto = command.ThirdPartyUser;
        var thirdPartyUser = await VerifyUserRepeatAsync(thirdPartyUserDto.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity, command.WhenExisReturn);
        if (thirdPartyUser is not null)
        {
            command.Result = thirdPartyUser.User.Adapt<UserModel>();
            return;
        }
        command.Result = await AddThirdPartyUserAsync(thirdPartyUserDto);
    }

    async Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserDto dto)
    {
        var thirdPartyUseEvent = new AddThirdPartyUserBeforeDomainEvent(dto.User);
        await _thirdPartyUserDomainService.AddBeforeAsync(thirdPartyUseEvent);
        var thirdPartyUser = new ThirdPartyUser(dto.ThirdPartyIdpId, thirdPartyUseEvent.Result.Id, true, dto.ThridPartyIdentity, dto.ExtendedData);
        await _thirdPartyUserRepository.AddAsync(thirdPartyUser);
        return thirdPartyUseEvent.Result;
    }

    [EventHandler(1)]
    public async Task UpsertThirdPartyUserExternalAsync(UpsertThirdPartyUserExternalCommand command)
    {
        var model = command.ThirdPartyUser;
        if (model.ThirdPartyIdpType == default)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_THIRD_PARTY_IDP_TYPE);
        }
        else if (model.ThirdPartyIdpType == ThirdPartyIdpTypes.Ldap)
        {
            var upsertThirdPartyUserForLdapCommand = new UpsertLdapUserCommand(
                    model.Id,
                    model.ThridPartyIdentity,
                    JsonSerializer.Serialize(model.ExtendedData),
                    model.Name,
                    model.DisplayName,
                    model.PhoneNumber!,
                    model.Email,
                    model.Account,
                    model.PhoneNumber!
                );
            await _eventBus.PublishAsync(upsertThirdPartyUserForLdapCommand);
            command.Result = upsertThirdPartyUserForLdapCommand.Result;
        }
        else
        {
            var identityProviderQuery = new IdentityProviderByTypeQuery(model.ThirdPartyIdpType);
            await _eventBus.PublishAsync(identityProviderQuery);
            var identityProvider = identityProviderQuery.Result;
            var thirdPartyUser = await VerifyUserRepeatAsync(identityProvider.Id, model.ThridPartyIdentity, false);
            if (thirdPartyUser is not null)
            {
                if (model.Id != default && thirdPartyUser.UserId != model.Id) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_FOUND);
                thirdPartyUser.Update(model.ThridPartyIdentity, JsonSerializer.Serialize(model.ExtendedData));
                await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
                var upsertUserCommand = new UpsertUserCommand(model.Adapt<UpsertUserModel>());
                await _eventBus.PublishAsync(upsertUserCommand);
                command.Result = upsertUserCommand.Result;
            }
            else
            {
                var addThirdPartyUserDto = new AddThirdPartyUserDto(identityProvider.Id, true, model.ThridPartyIdentity, JsonSerializer.Serialize(model.ExtendedData), command.ThirdPartyUser.Adapt<AddUserDto>());
                command.Result = await AddThirdPartyUserAsync(addThirdPartyUserDto);
            }
        }
    }

    [EventHandler(1)]
    public async Task UpdateThirdPartyUserAsync(UpdateThirdPartyUserCommand command)
    {
        var thirdPartyUserDto = command.ThirdPartyUser;
        var thirdPartyUser = await _thirdPartyUserRepository.FindAsync(tpu => tpu.Id == thirdPartyUserDto.Id);
        if (thirdPartyUser is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRD_PARTY_IDP_NOT_EXIST);

        await VerifyUserRepeatAsync(thirdPartyUser.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity);
        thirdPartyUser.Update(thirdPartyUserDto.Enabled, thirdPartyUserDto.ThridPartyIdentity, thirdPartyUserDto.ExtendedData);
        await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
    }

    [EventHandler(1)]
    public async Task UpsertLdapUserAsync(UpsertLdapUserCommand command)
    {
        var identityProviderQuery = new IdentityProviderByTypeQuery(ThirdPartyIdpTypes.Ldap);
        await _eventBus.PublishAsync(identityProviderQuery);
        var ldap = identityProviderQuery.Result;
        var thirdPartyUser = await VerifyUserRepeatAsync(ldap.Id, command.ThridPartyIdentity, false);
        if (thirdPartyUser is not null)
        {
            if (command.Id != default && thirdPartyUser.UserId != command.Id) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_FOUND);
            thirdPartyUser.Update(command.ThridPartyIdentity, command.ExtendedData);
            await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
            command.Result = (await _authDbContext.Set<User>().FirstAsync(u => u.Id == thirdPartyUser.UserId)).Adapt<UserModel>();
        }
        else
        {
            var addThirdPartyUserDto = new AddThirdPartyUserDto(ldap.Id, true, command.ThridPartyIdentity, command.ExtendedData, command.Adapt<AddUserDto>());
            command.Result = await AddThirdPartyUserAsync(addThirdPartyUserDto);
        }
        var upsertStaffDto = command.Adapt<UpsertStaffForLdapDto>();
        upsertStaffDto.UserId = command.Result.Id;
        var upsertStaffCommand = new UpsertStaffForLdapCommand(upsertStaffDto);
        await _eventBus.PublishAsync(upsertStaffCommand);
    }

    [EventHandler]
    public async Task AddThirdPartyUserExternalAsync(AddThirdPartyUserExternalCommand command)
    {
        var model = command.ThirdPartyUser;
        var identityProviderQuery = new IdentityProviderByTypeQuery(model.ThirdPartyIdpType);
        await _eventBus.PublishAsync(identityProviderQuery);
        var identityProvider = identityProviderQuery.Result;
        var addThirdPartyUserDto = model.Adapt<AddThirdPartyUserDto>();
        addThirdPartyUserDto.ThirdPartyIdpId = identityProvider.Id;
        var addThirdPartyUserCommand = new AddThirdPartyUserCommand(addThirdPartyUserDto, command.WhenExisReturn);
        await _eventBus.PublishAsync(addThirdPartyUserCommand);
        command.Result = addThirdPartyUserCommand.Result;
    }

    [EventHandler]
    public async Task RemoveThirdPartyUserAsync(RemoveThirdPartyUserCommand command)
    {
        await _thirdPartyUserRepository.RemoveAsync(tpu => tpu.ThirdPartyIdpId == command.ThirdPartyIdpId);
    }

    private async Task<ThirdPartyUser?> VerifyUserRepeatAsync(Guid thirdPartyIdpId, string thridPartyIdentity, bool throwException = true)
    {
        var thirdPartyUser = await _authDbContext.Set<ThirdPartyUser>()
                                                 .Include(tpu => tpu.User)
                                                 .ThenInclude(user => user.Roles)
                                                 .FirstOrDefaultAsync(tpu => tpu.ThirdPartyIdpId == thirdPartyIdpId && tpu.ThridPartyIdentity == thridPartyIdentity);
        if (thirdPartyUser != null && throwException)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.THIRD_PARTY_USER_EXIST, thridPartyIdentity);
        }
        return thirdPartyUser;
    }

    #endregion

    #region UserSystemData
    [EventHandler(1)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand command)
    {
        var data = command.UserSystemData;
        var item = await _userSystemBusinessDataRepository.FindAsync(userSystemBusinessData => userSystemBusinessData.UserId == data.UserId
        && userSystemBusinessData.SystemId == data.SystemId);
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
