// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class CommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAutoCompleteClient _autoCompleteClient;
    private readonly AuthDbContext _authDbContext;
    private readonly UserDomainService _userDomainService;
    private readonly AuthenticationDomainService _authenticationDomainService;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;
    private readonly ILogger<CommandHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Sms _sms;
    private readonly IMasaStackConfig _masaStackConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RoleDomainService _roleDomainService;
    private readonly IUserContext _userContext;
    private readonly IPasswordRuleProvider _passwordRuleProvider;

    public CommandHandler(
        IUserRepository userRepository,
        IAutoCompleteClient autoCompleteClient,
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        AuthenticationDomainService authenticationDomainService,
        IDistributedCacheClient distributedCacheClient,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository,
        ILogger<CommandHandler> logger,
        IEventBus eventBus,
        IHttpClientFactory httpClientFactory,
        Sms sms,
        IMasaStackConfig masaStackConfig,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        RoleDomainService roleDomainService,
        IUserContext userContext,
        IPasswordRuleProvider passwordRuleProvider)
    {
        _userRepository = userRepository;
        _autoCompleteClient = autoCompleteClient;
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _authenticationDomainService = authenticationDomainService;
        _distributedCacheClient = distributedCacheClient;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
        _logger = logger;
        _eventBus = eventBus;
        _httpClientFactory = httpClientFactory;
        _sms = sms;
        _masaStackConfig = masaStackConfig;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
        _roleDomainService = roleDomainService;
        _userContext = userContext;
        _passwordRuleProvider = passwordRuleProvider;
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
            ClientId = model.ClientId,
        });
        await _eventBus.PublishAsync(addUserCommand);
        command.Result = addUserCommand.Result.Adapt<UserModel>();
    }

    async Task RegisterVerifyAsync(RegisterByEmailModel model)
    {
        if (model.UserRegisterType == UserRegisterTypes.Undefined)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_REGISTER_TYPE);
        }

        if (model.UserRegisterType == UserRegisterTypes.Email)
        {
            var emailCodeKey = CacheKey.EmailCodeRegisterKey(model.Email);
            var emailCode = await _distributedCacheClient.GetAsync<string>(emailCodeKey);
            if (!string.Equals(model.EmailCode, emailCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_EMAIL_CAPTCHA);
            }
            await _distributedCacheClient.RemoveAsync(emailCodeKey);
        }

        if (model.UserRegisterType == UserRegisterTypes.PhoneNumber || !string.IsNullOrEmpty(model.PhoneNumber))
        {
            var smsCodeKey = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
            var smsCode = await _distributedCacheClient.GetAsync<string>(smsCodeKey);
            if (!string.Equals(model.SmsCode, smsCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SMS_CAPTCHA);
            }
            await _distributedCacheClient.RemoveAsync(smsCodeKey);
        }
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddUserCommand command)
    {
        var userDto = command.User;
        var user = new User(userDto.Id, userDto.Name, userDto.DisplayName, userDto.Avatar, userDto.IdCard, userDto.Account, userDto.Password, userDto.CompanyName, userDto.Department, userDto.Position, userDto.PhoneNumber, userDto.Landline, userDto.Email, userDto.Address, userDto.Gender, clientId: userDto.ClientId);
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
        if (command.User.Id == _userContext.GetUserId<Guid>())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CURRENT_USER_DELETE_ERROR);
        }

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
        var user = await _userRepository.AsQueryable()
            .Include(x => x.Roles).ThenInclude(x => x.Role)
            .Include(x => x.Staff)
            .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
        if (user is null)
        {
            var registerUserCommand = new RegisterUserCommand(new RegisterByEmailModel
            {
                PhoneNumber = model.PhoneNumber,
                SmsCode = model.Code,
                UserRegisterType = UserRegisterTypes.PhoneNumber,
                ClientId = ""
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

    [EventHandler(1)]
    public async Task LoginByEmailAsync(LoginByEmailCommand command)
    {
        var model = command.Model;
        var user = await _userRepository.AsQueryable()
            .Include(x => x.Roles).ThenInclude(x => x.Role)
            .Include(x => x.Staff)
            .FirstOrDefaultAsync(UserUniqueKeyQueries.EmailEquals(_authDbContext, model.Email));
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        var verifiedKey = await VerifyEmailCodeAsync(model.Email, model.Code);
        await _distributedCacheClient.RemoveAsync(verifiedKey);
        command.Result = _userDomainService.UserSplicingData(user);
    }

    /// <summary>
    /// Verify email login verification code (sent via SendEmailTypes.Login).
    /// Returns the cache key that matched.
    /// </summary>
    private async Task<string> VerifyEmailCodeAsync(string email, string code)
    {
        var key = CacheKey.EmailCodeLoginKey(email);
        var cachedCode = await _distributedCacheClient.GetAsync<string>(key);
        if (string.IsNullOrEmpty(cachedCode) || !string.Equals(code, cachedCode))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_EMAIL_CAPTCHA);
        }
        return key;
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
        var user = await _userRepository.FindAsync(UserUniqueKeyQueries.AccountEquals(_authDbContext, userModel.Account));
        if (user is null)
            throw new UserFriendlyException(UserFriendlyExceptionCodes.ACCOUNT_NOT_EXIST, userModel.Account);

        user.Disable();
        await _userDomainService.UpdateAsync(user);
        command.Result = true;
    }

    [EventHandler(1)]
    public async Task ValidateByAccountAsync(ValidateByAccountCommand validateByAccountCommand)
    {
        var model = validateByAccountCommand.ValidateAccountModel;
        validateByAccountCommand.Result = await _authenticationDomainService.ValidateByAccountAsync(model.Account, model.Password, model.LdapLogin);
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
        if (!string.Equals(command.Captcha, captcha))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_CAPTCHA);
        }
        await _distributedCacheClient.RemoveAsync(key);

        var user = await _userRepository.GetByVoucherAsync(command.Voucher);
        user.UpdatePassword(command.Password);
        await _userRepository.UpdateAsync(user);

        // clear lockout — user may have locked themselves out before resetting password
        await _distributedCacheClient.RemoveAsync<CacheLogin>(CacheKey.AccountLoginKey(user.Account));

        command.Result = user;
    }

    [EventHandler]
    public async Task LoginByAccountAsync(LoginByAccountCommand command)
    {
        //Swagger 免登录用的 ROPC 登录;走 IHttpClientFactory,避免每次请求新建连接池
        var httpClient = _httpClientFactory.CreateClient("SsoAddr");
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

        user.SetUserClaims(saveUserClaimValuesCommand.ClaimValues.Select(a => new KeyValuePair<string, string>(a.Key, a.Value)));

        await _userDomainService.UpdateAsync(user);
    }

    [EventHandler]
    public async Task SaveUserClaimValueAsync(SaveUserClaimValueCommand command)
    {
        var userClaimValue = await _authDbContext.Set<UserClaimValue>().FirstOrDefaultAsync(u => u.UserId == command.UserId && u.Name == command.ClaimName);

        if (userClaimValue is null)
        {
            userClaimValue = new UserClaimValue(command.ClaimName, command.ClaimValue)
            {
                UserId = command.UserId
            };

            await _authDbContext.Set<UserClaimValue>().AddAsync(userClaimValue);
        }
        else
        {
            userClaimValue.UpdateValue(command.ClaimValue);
        }

        await _authDbContext.SaveChangesAsync();
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

        command.Result = new ImpersonateOutput
        {
            ImpersonationToken = token
        };
    }

    [EventHandler]
    public async Task DeleteAccountAsync(DeleteAccountCommand command)
    {
        var userId = _userContext.GetUserId<Guid>();
        var user = await _userRepository.FindAsync(x => x.Id == userId);
        if (user is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        if (command.SmsCode is not null && command.EmailCode is not null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_CAPTCHA);
        }

        if (command.SmsCode is not null)
        {
            var smsCodeKey = CacheKey.MsgCodeDeleteAccountKey(user.PhoneNumber);
            var smsCode = await _distributedCacheClient.GetAsync<string>(smsCodeKey);
            if (!command.SmsCode.Equals(smsCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SMS_CAPTCHA);
            }
        }
        else if (command.EmailCode is not null)
        {
            var emailCodeKey = CacheKey.EmailCodeDeleteAccountKey(user.Email);
            var emailCode = await _distributedCacheClient.GetAsync<string>(emailCodeKey);
            if (!command.EmailCode.Equals(emailCode))
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_EMAIL_CAPTCHA);
            }
        }

        await _userDomainService.RemoveAsync(user.Id);
    }

    [EventHandler]
    public async Task GenerateNewPasswordAsync(GenerateNewPasswordCommand command)
    {
        command.Result = await _passwordRuleProvider.GenerateNewPasswordAsync();
    }

    [EventHandler(2)]
    public async Task ChangeStaffCurrentTeamAsync(UpdateStaffCurrentTeamCommand updateStaffCurrentTeamCommand)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == Guid.Empty)
        {
            _logger.LogError("User ID is empty when trying to change staff current team.");
            return;
        }

        //update claim value ,ensure update in token by IProfileService
        // 检查用户是否已经有相同的团队 Claim
        var existingClaimValue = await _authDbContext.Set<UserClaimValue>()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.Name == IdentityClaimConsts.CURRENT_TEAM);

        if (existingClaimValue != null)
        {
            // 如果已存在，更新值
            existingClaimValue.UpdateValue(updateStaffCurrentTeamCommand.TeamId.ToString());
        }
        else
        {
            // 如果不存在，创建新的 Claim
            var userClaimValue = new UserClaimValue(IdentityClaimConsts.CURRENT_TEAM, updateStaffCurrentTeamCommand.TeamId.ToString())
            {
                UserId = userId
            };
            await _authDbContext.Set<UserClaimValue>().AddAsync(userClaimValue);
        }

        await _authDbContext.SaveChangesAsync();
    }
}
