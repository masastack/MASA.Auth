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
    public async Task SendSMSAsync(SendSMSCommand command)
    {
        var model = command.Model;
        var cacheKey = "";
        switch (model.SendMsgCodeType)
        {
            case SendMsgCodeTypes.Register:
                if (_userRepository.Any(u => u.PhoneNumber == model.PhoneNumber))
                {
                    throw new UserFriendlyException($"This mobile phone number {model.PhoneNumber} already exists as a user");
                }
                cacheKey = CacheKey.MsgCodeForRegisterKey(model.PhoneNumber);
                break;
            case SendMsgCodeTypes.Login:
                var loginUser = await _userRepository.FindAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (loginUser == null)
                {
                    throw new UserFriendlyException($"User with mobile phone number {model.PhoneNumber} does not exist");
                }
                cacheKey = CacheKey.MsgCodeForLoginKey(loginUser.Id.ToString(), model.PhoneNumber);
                break;
            case SendMsgCodeTypes.VerifiyPhoneNumber:
                var user = await CheckUserExistAsync(model.UserId);
                ArgumentExceptionExtensions.ThrowIfNullOrEmpty(user.PhoneNumber);
                cacheKey = CacheKey.MsgCodeForVerifiyUserPhoneNumberKey(user.Id.ToString(), user.PhoneNumber);
                break;
            case SendMsgCodeTypes.UpdatePhoneNumber:
                await CheckUserExistAsync(model.UserId);
                cacheKey = CacheKey.MsgCodeForUpdateUserPhoneNumberKey(model.UserId.ToString(), model.PhoneNumber);
                break;
            default:
                throw new UserFriendlyException("Invalid SendMsgCodeType");
        }
        var alreadySend = await _sms.CheckAlreadySendAsync(cacheKey);
        if (alreadySend)
        {
            throw new UserFriendlyException("Verification code has been sent, please try again later");
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
                    throw new UserFriendlyException($"This email {model.Email} already exists as a user");
                }
                break;
            case SendEmailTypes.Verifiy:
            case SendEmailTypes.ForgotPassword:
                throw new NotImplementedException();
            default:
                throw new UserFriendlyException("Invalid SendEmailType");
        }
        await _emailAgent.SendEmailAsync(model);
    }

    async Task<User> CheckUserExistAsync(Guid userId)
    {
        var user = await _userRepository.FindAsync(u => u.Id == userId);
        return user ?? throw new UserFriendlyException("The current user does not exist");
    }
}
