// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAutoCompleteClient _autoCompleteClient;
    private readonly AuthDbContext _authDbContext;
    private readonly UserDomainService _userDomainService;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;
    private readonly ILdapFactory _ldapFactory;
    private readonly ILdapIdpRepository _ldapIdpRepository;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly Sms _sms;
    private readonly IMasaStackConfig _masaStackConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMasaConfiguration _masaConfiguration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LdapDomainService _ldapDomainService;
    private readonly RoleDomainService _roleDomainService;
    private readonly IUserContext _userContext;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        IDistributedCacheClient distributedCacheClient,
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
        LdapDomainService ldapDomainService,
        RoleDomainService roleDomainService,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _distributedCacheClient = distributedCacheClient;
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
        _ldapDomainService = ldapDomainService;
        _roleDomainService = roleDomainService;
        _userContext = userContext;
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
        var smsCodeKey = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
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
        var user = new User(userDto.Id, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Gender);
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
        user.Update(userDto.Account, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.CompanyName, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Department, userDto.Position, userDto.Gender);
        if (userDto.Enabled)
        {
            user.Enable();
        }
        else
        {
            user.Disable();
        }
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

        var oldRoleIds = user.Roles.Select(r => r.RoleId);

        user.SetRoles(userDto.Roles);
        user.AddPermissions(userDto.Permissions);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        await _roleDomainService.UpdateRoleLimitAsync(oldRoleIds.Except(userDto.Roles));
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
        user.UpdateBasicInfo(userModel.Name, userModel.DisplayName, userModel.Gender, userModel.Avatar, userModel.CompanyName, userModel.Department, userModel.Position, new AddressValue(userModel.Address.Address, "", "", ""));
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
            user.SetRoles(roles.Union(user.Roles.Select(role => role.RoleId)));
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
                case SendMsgCodeTypes.Register:
                    msgCodeKey = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
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
            var registerUserCommand = new RegisterUserCommand(new RegisterByEmailModel
            {
                PhoneNumber = model.PhoneNumber,
                SmsCode = model.Code,
                UserRegisterType = UserRegisterTypes.PhoneNumber
            });
            await _eventBus.PublishAsync(registerUserCommand);
            command.Result = new UserDetailDto
            {
                Id = registerUserCommand.Result.Id,
                Account = registerUserCommand.Result.Account,
                PhoneNumber = registerUserCommand.Result.PhoneNumber,
                Avatar = registerUserCommand.Result.Avatar,
                CreationTime = registerUserCommand.Result.CreationTime,
                DisplayName = registerUserCommand.Result.DisplayName,
                Name = registerUserCommand.Result.Name,
                Enabled = registerUserCommand.Result.Enabled,
                Gender = registerUserCommand.Result.Gender
            };
            return;
        }
        var key = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
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

        user.Disable();
        await _userDomainService.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler(1)]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        //TODO UserDomainService
        var account = validateByAccountCommand.ValidateAccountModel.Account;
        var password = validateByAccountCommand.ValidateAccountModel.Password;

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

        var isLdap = validateByAccountCommand.ValidateAccountModel.LdapLogin;
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

            account = await _ldapDomainService.UpsertLdapUserAsync(ldapUser);
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
        var user = await _userRepository.GetDetailAsync(userId);
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

    [EventHandler]
    public async Task SaveUserClaimValuesAsync(SaveUserClaimValuesCommand saveUserClaimValuesCommand)
    {
        var user = await _authDbContext.Set<User>()
                                            .Include(u => u.UserClaims)
                                            .FirstOrDefaultAsync(u => u.Id == saveUserClaimValuesCommand.UserId);
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        user.UserClaimValues(saveUserClaimValuesCommand.ClaimValues);

        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler]
    public async Task ImpersonateAsync(ImpersonateUserCommand command)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == default)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }
        var cacheItem = new ImpersonationCacheItem(
                command.UserId,
                command.IsBackToImpersonator
            );

        if (!command.IsBackToImpersonator)
        {
            cacheItem.ImpersonatorUserId = userId;
        }

        var token = Guid.NewGuid().ToString();
        var key = CacheKey.ImpersonationUserKey(token);
        await _distributedCacheClient.SetAsync(key, cacheItem, TimeSpan.FromMinutes(10));

        command.Result = new ImpersonateOutput {
            ImpersonationToken = token
        };
    }
}
