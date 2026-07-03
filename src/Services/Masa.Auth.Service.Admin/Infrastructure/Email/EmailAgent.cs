// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Email;

public class EmailAgent : IScopedDependency
{
    readonly IMcClient _mcClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IMasaConfiguration _masaConfiguration;
    readonly IClientMessageTemplateProvider _clientMessageTemplateProvider;

    public EmailAgent(
        IMcClient mcClient,
        IDistributedCacheClient distributedCacheClient,
        IMasaConfiguration masaConfiguration,
        IClientMessageTemplateProvider clientMessageTemplateProvider)
    {
        _mcClient = mcClient;
        _distributedCacheClient = distributedCacheClient;
        _masaConfiguration = masaConfiguration;
        _clientMessageTemplateProvider = clientMessageTemplateProvider;
    }

    public async Task SendEmailAsync(SendEmailModel sendEmailModel, string? clientId = null, TimeSpan? expiration = null)
    {
        //todo Abstract Factory
        var sendKey = string.Empty;
        var sendValueKey = string.Empty;

        switch (sendEmailModel.SendEmailType)
        {
            case SendEmailTypes.Verifiy:
                sendKey = CacheKey.EmailCodeVerifiySendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeVerifiyKey(sendEmailModel.Email);
                break;
            case SendEmailTypes.Register:
                sendKey = CacheKey.EmailCodeRegisterSendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeRegisterKey(sendEmailModel.Email);
                break;
            case SendEmailTypes.ForgotPassword:
                sendKey = CacheKey.EmailCodeForgotPasswordSendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeForgotPasswordKey(sendEmailModel.Email);
                break;
            case SendEmailTypes.UpdateEmail:
                sendKey = CacheKey.EmailCodeUpdateSendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeUpdateKey(sendEmailModel.Email);
                break;
            case SendEmailTypes.Bind:
                sendKey = CacheKey.EmailCodeBindSendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeBindKey(sendEmailModel.Email);
                break;
            case SendEmailTypes.DeleteAccount:
                sendKey = CacheKey.EmailCodeDeleteAccountSendKey(sendEmailModel.Email);
                sendValueKey = CacheKey.EmailCodeDeleteAccountKey(sendEmailModel.Email);
                break;
            default:
                break;
        }

        if (await CheckAlreadySendAsync(sendKey))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.EMAIL_SENDED);
        }

        var channelCode = "";
        var templateCode = "";
        var clientTemplate = await _clientMessageTemplateProvider.ResolveAsync(clientId, ChannelTypes.Email, (int)sendEmailModel.SendEmailType);
        if (clientTemplate is not null)
        {
            (channelCode, templateCode) = clientTemplate.Value;
        }
        else
        {
            var _emailOptions = _masaConfiguration.ConfigurationApi.GetPublic().GetSection(EmailOptions.Key).Get<EmailOptions>();
            MasaArgumentException.ThrowIfNull(_emailOptions, nameof(EmailOptions));
            channelCode = _emailOptions.ChannelCode;
            templateCode = _emailOptions.TemplateCode;
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        await _mcClient.MessageTaskService.SendTemplateMessageByExternalAsync(new SendTemplateMessageByExternalModel
        {
            ChannelCode = channelCode,
            ChannelType = ChannelTypes.Email,
            TemplateCode = templateCode,
            ReceiverType = SendTargets.Assign,
            Receivers = new List<ExternalReceiverModel>
            {
                new ExternalReceiverModel
                {
                    ChannelUserIdentity = sendEmailModel.Email
                }
            },
            Variables = new ExtraPropertyDictionary(new Dictionary<string, object>
            {
                ["code"] = code,
            })
        });

        var emailCaptchaExpired = _masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue<int>("AppSettings:EmailCaptchaExpired", 300);
        await _distributedCacheClient.SetAsync(sendValueKey, code, expiration ?? TimeSpan.FromSeconds(emailCaptchaExpired));
        await _distributedCacheClient.SetAsync(sendKey, true, expiration ?? TimeSpan.FromSeconds(60));
    }

    public async Task<bool> VerifyCodeAsync(string key, string code)
    {
        var codeCache = await _distributedCacheClient.GetAsync<string>(key);
        var result = codeCache != code;
        if (result)
        {
            await _distributedCacheClient.RemoveAsync(key);
        }
        return result;
    }

    async Task<bool> CheckAlreadySendAsync(string key)
    {
        return await _distributedCacheClient.ExistsAsync(key);
    }
}
