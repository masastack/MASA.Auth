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
    readonly IMasaStackConfig _masaStackConfig;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IMasaConfiguration _masaConfiguration;
    readonly IUnitOfWork _unitOfWork;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        IStaffRepository staffRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        AuthDbContext authDbContext,
        StaffDomainService staffDomainService,
        UserDomainService userDomainService,
        IMultilevelCacheClient multilevelCacheClient,
        IDistributedCacheClient distributedCacheClient,
        IUserContext userContext,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository,
        ILdapFactory ldapFactory,
        ILdapIdpRepository ldapIdpRepository,
        ILogger<CommandHandler> logger,
        IEventBus eventBus,
        Sms sms,
        IMasaStackConfig masaStackConfig,
        IHttpContextAccessor httpContextAccessor,
        IMasaConfiguration masaConfiguration,
        IUnitOfWork unitOfWork,
        ThirdPartyUserDomainService thirdPartyUserDomainService)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _staffRepository = staffRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _authDbContext = authDbContext;
        _staffDomainService = staffDomainService;
        _userDomainService = userDomainService;
        _multilevelCacheClient = multilevelCacheClient;
        _distributedCacheClient = distributedCacheClient;
        _userContext = userContext;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
        _ldapFactory = ldapFactory;
        _ldapIdpRepository = ldapIdpRepository;
        _logger = logger;
        _eventBus = eventBus;
        _sms = sms;
        _masaStackConfig = masaStackConfig;
        _httpContextAccessor = httpContextAccessor;
        _masaConfiguration = masaConfiguration;
        _unitOfWork = unitOfWork;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
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
        var user = new User(userDto.Id, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Gender);
        user.SetRoles(userDto.Roles);
        user.AddPermissions(userDto.Permissions);
        await _userDomainService.AddAsync(user);
        command.Result = user;
    }

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateUserCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        user.Update(userDto.Account, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.Enabled, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
        await _userDomainService.UpdateAsync(user);
        command.Result = user;
    }

    [EventHandler(1)]
    public async Task RemoveUserAsync(RemoveUserCommand command)
    {
        await _userDomainService.RemoveAsync(command.User.Id);
    }

    [EventHandler(1)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand command)
    {
        var userDto = command.User;
        var user = await _userRepository.GetDetailAsync(userDto.Id);
        user.SetRoles(userDto.Roles);
        user.AddPermissions(userDto.Permissions);
        await _userRepository.UpdateAsync(user);
        await _userDomainService.UpdateAuthorizationAsync(userDto.Roles);
        await _unitOfWork.SaveChangesAsync();
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
        if (!user.VerifyPassword(userModel.OldPassword ?? ""))
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
        user.UpdateBasicInfo(userModel.Name, userModel.DisplayName, userModel.Gender, userModel.CompanyName, userModel.Department, userModel.Position, new AddressValue(userModel.Address.Address, "", "", ""));
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
            user = await _authDbContext.Set<User>()
                                       .Include(u => u.Roles)
                                       .FirstOrDefaultAsync(u => u.Id == userModel.Id);
            if (user is null)
            {
                throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_NOT_FOUND);
            }
        }

        if (user != null)
        {
            user.Update(userModel.Name, userModel.DisplayName!, userModel.IdCard, userModel.CompanyName, userModel.Department, userModel.Gender);
            roles.AddRange(user.Roles.Select(role => role.RoleId));
            user.SetRoles(roles);
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

    [EventHandler]
    public async Task UpdateUserAvatarAsync(UpdateUserAvatarCommand command)
    {
        var userDto = command.User;
        var user = await CheckUserExistAsync(userDto.Id);
        user.UpdateAvatar(userDto.Avatar);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler]
    public async Task VerifyMsgCodeForVerifiyPhoneNumberAsync(VerifyMsgCodeCommand command)
    {
        var model = command.Model;
        var msgCodeKey = "";
        if (model.SendMsgCodeType == SendMsgCodeTypes.VerifiyPhoneNumber)
        {
            var user = await CheckUserExistAsync(model.UserId);
            msgCodeKey = CacheKey.MsgCodeForVerifiyUserPhoneNumberKey(model.UserId.ToString(), user.PhoneNumber);
            if (await _sms.VerifyMsgCodeAsync(msgCodeKey, model.Code))
            {
                var resultKey = CacheKey.VerifiyUserPhoneNumberResultKey(user.Id.ToString(), user.PhoneNumber);
                await _distributedCacheClient.SetAsync(resultKey, true, TimeSpan.FromSeconds(60 * 10));
                command.Result = true;
            }
        }
        else
        {
            ArgumentExceptionExtensions.ThrowIfNullOrEmpty(model.PhoneNumber);
            switch (model.SendMsgCodeType)
            {
                case SendMsgCodeTypes.UpdatePhoneNumber:
                    msgCodeKey = CacheKey.MsgCodeForUpdateUserPhoneNumberKey(model.UserId.ToString(), model.PhoneNumber);
                    break;
                case SendMsgCodeTypes.Login:
                    msgCodeKey = CacheKey.MsgCodeForLoginKey(model.UserId.ToString(), model.PhoneNumber);
                    break;
                case SendMsgCodeTypes.Register:
                    msgCodeKey = CacheKey.MsgCodeForRegisterKey(model.PhoneNumber);
                    break;
                case SendMsgCodeTypes.Bind:
                    msgCodeKey = CacheKey.MsgCodeForBindKey(model.PhoneNumber);
                    break;
                case SendMsgCodeTypes.ForgotPassword:
                    msgCodeKey = CacheKey.MsgCodeForgotPasswordKey(model.PhoneNumber);
                    break;
                default:
                    break;
            }

            if (await _sms.VerifyMsgCodeAsync(msgCodeKey, model.Code, false))
            {
                command.Result = true;
            }
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
                user.UpdatePhoneNumber(userDto.PhoneNumber);
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
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
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
        command.Result = _userDomainService.UserSplicingData(user);
    }

    [EventHandler]
    public async Task RemoveUserRolesAsync(RemoveUserRolesCommand command)
    {
        var userModel = command.User;
        var user = await _authDbContext.Set<User>()
                                 .Include(u => u.Roles)
                                 .AsTracking()
                                 .FirstAsync(u => u.Id == userModel.Id);
        var roleIds = await _authDbContext.Set<Role>()
                                    .Where(role => userModel.RoleCodes.Contains(role.Code))
                                    .Select(role => role.Id)
                                    .ToListAsync();
        user.RemoveRoles(roleIds);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler(1)]
    public async Task DisableUserAsync(DisableUserCommand command)
    {
        var userModel = command.User;
        var user = await _userRepository.FindAsync(u => u.Account == userModel.Account);
        if (user is null)
            throw new UserFriendlyException(UserFriendlyExceptionCodes.ACCOUNT_NOT_EXIST, userModel.Account);

        user.Disabled();
        await _userDomainService.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler(1)]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //TODO UserDomainService
        var account = validateByAccountCommand.UserAccountValidateDto.Account;
        var password = validateByAccountCommand.UserAccountValidateDto.Password;

        var loginFreeze = _masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue("AppSettings:LoginFreeze", false);
        var key = CacheKey.AccountLoginKey(account);
        CacheLogin? loginCache = null;
        if (loginFreeze)
        {
            loginCache = await _distributedCacheClient.GetAsync<CacheLogin>(key);
            if (loginCache is not null && loginCache.LoginErrorCount >= 5)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LOGIN_FREEZE);
            }
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

        var user = await _userRepository.FindWithIncludAsync(u => EF.Functions.Collate(u.Account, "SQL_Latin1_General_CP1_CS_AS") == account || u.PhoneNumber == account, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}",nameof(User.Staff)
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
                if (loginFreeze)
                {
                    loginCache ??= new() { FreezeTime = DateTimeOffset.Now.AddMinutes(30), Account = account };
                    loginCache.LoginErrorCount++;
                    await _distributedCacheClient.SetAsync(key, loginCache, loginCache.FreezeTime);
                }
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PASSWORD_FAILED);
            }

            if (loginCache is not null)
            {
                await _distributedCacheClient.RemoveAsync<CacheLogin>(key);
            }
        }

        validateByAccountCommand.Result = _userDomainService.UserSplicingData(user);
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
        var docUrl = _masaStackConfig.GetSsoDomain();
#if DEBUG
        docUrl = "http://localhost:18200";
#endif
        var disco = await httpClient.GetDiscoveryDocumentAsync(docUrl);
        var loginResult = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _masaStackConfig.GetWebId(MasaStackProject.Auth),
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

    [EventHandler]
    public async Task BindRolesAsync(BindUserRolesCommand command)
    {
        var userModel = command.User;
        if (!userModel.RoleCodes.Any()) return;

        var user = await _authDbContext.Set<User>()
                                    .Include(u => u.Roles)
                                    .FirstOrDefaultAsync(u => u.Id == userModel.Id);
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        var roles = await _authDbContext.Set<Role>()
                                    .Where(role => userModel.RoleCodes.Contains(role.Code))
                                    .Select(role => role.Id)
                                    .ToListAsync();
        user.AddRoles(roles);
        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler]
    public async Task UnbindRolesAsync(UnbindUserRolesCommand command)
    {
        var userModel = command.User;
        if (!userModel.RoleCodes.Any()) return;

        var user = await _authDbContext.Set<User>()
                                    .Include(u => u.Roles)
                                    .FirstOrDefaultAsync(u => u.Id == userModel.Id);
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        var roles = await _authDbContext.Set<Role>()
                                    .Where(role => userModel.RoleCodes.Contains(role.Code))
                                    .Select(role => role.Id)
                                    .ToListAsync();
        user.RemoveRoles(roles);
        await _userDomainService.UpdateAsync(user);
    }

    #endregion

    #region Staff

    [EventHandler(1)]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        command.Result = await _staffDomainService.UpdateAsync(command.Staff);
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

        staff.UpdateBasicInfo(staffModel.Name, staffModel.DisplayName, staffModel.Gender, staffModel.PhoneNumber, staffModel.Email);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpsertStaffForLdapAsync(UpsertStaffForLdapCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == staffDto.UserId);
        if (staff is not null)
        {
            command.Result = await _staffDomainService.UpdateAsync(new UpdateStaffDto
            {
                Id = staff.Id,
                Name = staffDto.Name,
                DisplayName = staffDto.DisplayName,
                PhoneNumber = staffDto.PhoneNumber,
                Email = staffDto.Email
            });
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
            await _staffDomainService.VerifyRepeatAsync(addStaffDto.JobNumber, addStaffDto.PhoneNumber, addStaffDto.Email, addStaffDto.IdCard);
            command.Result = await _staffDomainService.AddAsync(addStaffDto);
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
        command.Result = await _staffDomainService.RemoveAsync(command.Staff.Id);
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
                    Errors = result.Errors.GroupBy(error => error.PropertyName).Select(e => e.First().ErrorMessage).ToList()
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
                var (existStaff, _) = await _staffDomainService.VerifyRepeatAsync(syncStaff.JobNumber, syncStaff.PhoneNumber, syncStaff.Email, syncStaff.IdCard);
                if (existStaff is not null)
                {
                    var checkResult = await CheckSyncDataAsync(existStaff, syncStaff);
                    if (!checkResult.State)
                    {
                        if (checkResult.StaffResult != null)
                        {
                            syncResults[i] = checkResult.StaffResult;
                        }
                        continue;
                    }
                    await _staffDomainService.UpdateAsync(new UpdateStaffDto
                    {
                        Id = existStaff.Id,
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Email = syncStaff.Email,
                        IdCard = syncStaff.IdCard,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                        Position = syncStaff.Position
                    });
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
                    await _staffDomainService.AddAsync(addStaffDto);
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
            if (syncStaffs.Where(staff => string.IsNullOrEmpty(func(staff)) is false).IsDuplicate(func, out List<SyncStaffDto>? duplicates))
            {
                foreach (var duplicate in duplicates)
                {
                    var index = syncStaffs.IndexOf(duplicate);
                    var staff = syncStaffs[index];
                    syncResults[index] = new()
                    {
                        JobNumber = staff.JobNumber,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }

        SyncStaffResultsDto.SyncStaffResult Error(string jobNumber, params string[] errorMessages) =>
            new SyncStaffResultsDto.SyncStaffResult()
            {
                JobNumber = jobNumber,
                Errors = errorMessages.ToList()
            };

        async Task<(bool State, SyncStaffResultsDto.SyncStaffResult? StaffResult)> CheckSyncDataAsync(Staff existStaff,
            SyncStaffDto syncStaff)
        {
            // Do not flush to db if the sync staff info is same with exist staff. 
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber &&
                existStaff.JobNumber == syncStaff.JobNumber &&
                existStaff.DisplayName == syncStaff.DisplayName.WhenNullOrEmptyReplace("") &&
                existStaff.Name == syncStaff.Name.WhenNullOrEmptyReplace("") &&
                existStaff.Gender == syncStaff.Gender &&
                existStaff.StaffType == syncStaff.StaffType &&
                existStaff.Position != null &&
                existStaff.Position.Name == syncStaff.Position.WhenNullOrEmptyReplace("") &&
                existStaff.Email == syncStaff.Email.WhenNullOrEmptyReplace("") &&
                existStaff.IdCard == syncStaff.IdCard.WhenNullOrEmptyReplace(""))
            {
                return (false, default);
            }

            // When phone number is not equals and job number is not equals, the email and id card cannot be equal! 
            if (existStaff.PhoneNumber != syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.Email) && existStaff.Email == syncStaff.Email)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard) && existStaff.IdCard == syncStaff.IdCard)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose id card is {syncStaff.IdCard} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }
            }

            // When job number is equals, the phone number must be equal
            if (existStaff.JobNumber == syncStaff.JobNumber && existStaff.PhoneNumber != syncStaff.PhoneNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose job number is {syncStaff.JobNumber}, the corresponding mobile phone number is {existStaff.PhoneNumber}, which does not match the mobile phone number {syncStaff.PhoneNumber}"));
            }

            // When phone number is equals, the job number must be equal
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose mobile phone number is {syncStaff.PhoneNumber} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
            }

            // When job number is equal and phone number is equal, the staff whose is email or id card cannot be equal for other staff who is in database.
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber == syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard))
                {
                    var (existIdCardStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, default, syncStaff.IdCard);
                    if (existIdCardStaff != null && existIdCardStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose id card number is {syncStaff.IdCard} has a corresponding job number of {existIdCardStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.Email))
                {
                    var (existEmailStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, syncStaff.Email, default);
                    if (existEmailStaff != null && existEmailStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existEmailStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }
            }

            return (true, default);
        }
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
            Scheme = model.Scheme,
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

    async Task BindVerifyAsync(RegisterThirdPartyUserModel model)
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

        Expression<Func<User, bool>> condition = _ => false;
        condition = condition.Or(!model.PhoneNumber.IsNullOrEmpty(), u => u.PhoneNumber == model.PhoneNumber);
        condition = condition.Or(!model.Email.IsNullOrEmpty(), u => u.Email == model.Email);
        var user = await _userRepository.FindAsync(condition);
        if (user != null)
        {
            var identityProviderQuery = new IdentityProviderBySchemeQuery(model.Scheme);
            await _eventBus.PublishAsync(identityProviderQuery);
            var identityProvider = identityProviderQuery.Result;
            if (identityProvider != null)
            {
                var thirdPartyUser = await _thirdPartyUserRepository.FindAsync(t => t.UserId == user.Id && t.ThirdPartyIdpId == identityProvider.Id);
                if (thirdPartyUser != null)
                {
                    throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.THIRDPARTYUSER_BIND_EXIST);
                }
            }
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

        command.Result = await _thirdPartyUserDomainService.AddThirdPartyUserAsync(thirdPartyUserDto);
    }

    [EventHandler(1)]
    public async Task UpsertThirdPartyUserExternalAsync(UpsertThirdPartyUserExternalCommand command)
    {
        var model = command.ThirdPartyUser;
        if (string.IsNullOrEmpty(model.Scheme))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_THIRD_PARTY_IDP_TYPE);
        }
        else if (string.Equals(model.Scheme, LdapConsts.LDAP_NAME, StringComparison.OrdinalIgnoreCase))
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
            var identityProviderQuery = new IdentityProviderBySchemeQuery(model.Scheme);
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
                command.Result = await _thirdPartyUserDomainService.AddThirdPartyUserAsync(addThirdPartyUserDto);
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
        thirdPartyUser.Update(thirdPartyUserDto.ThridPartyIdentity, thirdPartyUserDto.ExtendedData);
        if (thirdPartyUserDto.Enabled)
        {
            thirdPartyUser.Enable();
        }
        else
        {
            thirdPartyUser.Disable();
        }
        await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
    }

    [EventHandler(1)]
    public async Task UpsertLdapUserAsync(UpsertLdapUserCommand command)
    {
        var identityProviderQuery = new IdentityProviderByTypeQuery(ThirdPartyIdpTypes.Ldap);
        await _eventBus.PublishAsync(identityProviderQuery);
        var ldap = identityProviderQuery.Result;
        var thirdPartyUser = await VerifyUserRepeatAsync(ldap.Id, command.ThridPartyUserIdentity, false);
        if (thirdPartyUser is not null)
        {
            if (command.Id != default && thirdPartyUser.UserId != command.Id) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_FOUND);
            thirdPartyUser.Update(command.ThridPartyUserIdentity, command.ExtendedData);
            await _thirdPartyUserRepository.UpdateAsync(thirdPartyUser);
            command.Result = (await _authDbContext.Set<User>().FirstAsync(u => u.Id == thirdPartyUser.UserId)).Adapt<UserModel>();
        }
        else
        {
            var addThirdPartyUserDto = new AddThirdPartyUserDto(ldap.Id, true, command.ThridPartyUserIdentity, command.ExtendedData, command.Adapt<AddUserDto>());
            command.Result = await _thirdPartyUserDomainService.AddThirdPartyUserAsync(addThirdPartyUserDto);
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
        var identityProviderQuery = new IdentityProviderBySchemeQuery(model.Scheme);
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
