// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class ThirdPartyCommandHandler
{
    readonly IUserRepository _userRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly AuthDbContext _authDbContext;
    readonly ThirdPartyUserDomainService _thirdPartyUserDomainService;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IEventBus _eventBus;

    public ThirdPartyCommandHandler(
        IUserRepository userRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        AuthDbContext authDbContext,
        IDistributedCacheClient distributedCacheClient,
        IEventBus eventBus,
        ThirdPartyUserDomainService thirdPartyUserDomainService)
    {
        _userRepository = userRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _authDbContext = authDbContext;
        _distributedCacheClient = distributedCacheClient;
        _eventBus = eventBus;
        _thirdPartyUserDomainService = thirdPartyUserDomainService;
    }

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
        var (thirdPartyUser, exception) = await _thirdPartyUserDomainService.VerifyRepeatAsync(thirdPartyUserDto.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity);

        if (command.WhenExisReturn && thirdPartyUser != null)
        {
            command.Result = thirdPartyUser.User.Adapt<UserModel>();
            return;
        }
        if (exception is not null)
        {
            throw exception;
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
        else
        {
            var identityProviderQuery = new IdentityProviderBySchemeQuery(model.Scheme);
            await _eventBus.PublishAsync(identityProviderQuery);
            var identityProvider = identityProviderQuery.Result;
            var (thirdPartyUser, _) = await _thirdPartyUserDomainService.VerifyRepeatAsync(identityProvider.Id, model.ThridPartyIdentity);
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
                var addThirdPartyUserDto = new AddThirdPartyUserDto(identityProvider.Id, true, model.ThridPartyIdentity, JsonSerializer.Serialize(model.ExtendedData), command.ThirdPartyUser.Adapt<AddUserDto>(), model.ClaimData);
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

        var (_, exception) = await _thirdPartyUserDomainService.VerifyRepeatAsync(thirdPartyUser.ThirdPartyIdpId, thirdPartyUserDto.ThridPartyIdentity);
        if (exception is not null)
        {
            throw exception;
        }
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

    [EventHandler]
    public async Task RemoveThirdPartyUserByIdAsync(RemoveThirdPartyUserByIdCommand command)
    {
        await _thirdPartyUserRepository.RemoveAsync(tpu => tpu.Id == command.Id);
    }

    [EventHandler]
    public async Task RemoveThirdPartyUserByThridPartyIdentityAsync(RemoveThirdPartyUserByThridPartyIdentityCommand command)
    {
        await _thirdPartyUserRepository.RemoveAsync(tpu => tpu.ThridPartyIdentity == command.ThridPartyIdentity);
    }

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
}
