// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Messages;

public class CommandHandler
{
    readonly Sms _sms;
    readonly EmailAgent _emailAgent;
    readonly IUserRepository _userRepository;

    public CommandHandler(
        Sms sms,
        EmailAgent emailAgent,
        IUserRepository userRepository)
    {
        _sms = sms;
        _emailAgent = emailAgent;
        _userRepository = userRepository;
    }

    [EventHandler]
    public async Task SendSmsAsync(SendSmsCommand command)
    {
        var model = command.Model;
        var cacheKey = "";
        switch (model.SendMsgCodeType)
        {
            case SendMsgCodeTypes.Register:
                if (_userRepository.Any(u => u.PhoneNumber == model.PhoneNumber))
                {
                    throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_EXIST, model.PhoneNumber);
                }
                cacheKey = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
                break;
            case SendMsgCodeTypes.Bind:
                cacheKey = CacheKey.MsgCodeForBindKey(model.PhoneNumber);
                break;
            case SendMsgCodeTypes.Login:
                cacheKey = CacheKey.MsgCodeRegisterAndLoginKey(model.PhoneNumber);
                break;
            case SendMsgCodeTypes.VerifiyPhoneNumber:
                var user = await CheckUserExistAsync(model.UserId);
                ArgumentExceptionExtensions.ThrowIfNullOrEmpty(user.PhoneNumber);
                model.PhoneNumber = user.PhoneNumber;
                cacheKey = CacheKey.MsgCodeForVerifiyUserPhoneNumberKey(user.Id.ToString(), user.PhoneNumber);
                break;
            case SendMsgCodeTypes.UpdatePhoneNumber:
                await CheckUserExistAsync(model.UserId);
                cacheKey = CacheKey.MsgCodeForUpdateUserPhoneNumberKey(model.UserId.ToString(), model.PhoneNumber);
                break;
            case SendMsgCodeTypes.ForgotPassword:
                if (_userRepository.Any(u => u.PhoneNumber == model.PhoneNumber) is false)
                {
                    throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_PHONE_NUMBER_NOT_EXIST, model.PhoneNumber);
                }
                cacheKey = CacheKey.MsgCodeForgotPasswordKey(model.PhoneNumber);
                break;
            case SendMsgCodeTypes.DeleteAccount:
                cacheKey = CacheKey.MsgCodeDeleteAccountKey(model.PhoneNumber);
                break;
            default:
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SEND_MSG_CODE_TYPE);
        }
        var alreadySend = await _sms.CheckAlreadySendAsync(cacheKey);
        if (alreadySend)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CAPTCHA_SENDED);
        }
        else
        {
            await _sms.SendMsgCodeAsync(cacheKey, model.PhoneNumber);
        }
    }

    [EventHandler]
    public async Task SendEmailAsync(SendEmailCommand command)
    {
        var model = command.SendEmailModel;
        switch (model.SendEmailType)
        {
            case SendEmailTypes.Register:
                if (_userRepository.Any(u => u.Email == model.Email))
                {
                    throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_EXIST, model.Email);
                }
                break;
            case SendEmailTypes.Verifiy:
            case SendEmailTypes.UpdateEmail:
            case SendEmailTypes.ForgotPassword:
                if (!_userRepository.Any(u => u.Email == model.Email))
                {
                    throw new UserFriendlyException(UserFriendlyExceptionCodes.USER_EMAIL_NOT_EXIST, model.Email);
                }
                break;
            case SendEmailTypes.Bind:
                break;
            default:
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.INVALID_SEND_EMAIL_TYPE);
        }
        await _emailAgent.SendEmailAsync(model);
    }

    async Task<User> CheckUserExistAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        return user ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
    }
}
