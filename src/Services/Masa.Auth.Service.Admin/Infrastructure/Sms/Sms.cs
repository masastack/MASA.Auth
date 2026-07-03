// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Sms;

public class Sms : IScopedDependency
{
    readonly IMcClient _mcClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IMasaConfiguration _masaConfiguration;
    readonly IClientMessageTemplateProvider _clientMessageTemplateProvider;

    public Sms(
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

    public async Task<string> SendMsgCodeAsync(string key, string phoneNumber, string? clientId = null, SendMsgCodeTypes scene = default, TimeSpan? expiration = null)
    {
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(phoneNumber);

        // Client-level template takes precedence; fall back to the global DCC SmsOptions.
        var channelCode = "";
        var templateCode = "";
        var clientTemplate = await _clientMessageTemplateProvider.ResolveAsync(clientId, ChannelTypes.Sms, (int)scene);
        if (clientTemplate is not null)
        {
            (channelCode, templateCode) = clientTemplate.Value;
        }
        else
        {
            var _smsOptions = _masaConfiguration.ConfigurationApi.GetPublic().GetSection(SmsOptions.Key).Get<SmsOptions>();
            MasaArgumentException.ThrowIfNull(_smsOptions, nameof(SmsOptions));
            channelCode = _smsOptions.ChannelCode;
            templateCode = _smsOptions.TemplateCode;
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        await _mcClient.MessageTaskService.SendTemplateMessageByExternalAsync(new SendTemplateMessageByExternalModel
        {
            ChannelCode = channelCode,
            ChannelType = ChannelTypes.Sms,
            TemplateCode = templateCode,
            ReceiverType = SendTargets.Assign,
            Receivers = new List<ExternalReceiverModel>
            {
                new ExternalReceiverModel
                {
                    ChannelUserIdentity = phoneNumber
                }
            },
            Variables = new ExtraPropertyDictionary(new Dictionary<string, object>
            {
                ["code"] = code,
            })
        });
        var smsCaptchaExpired = _masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue<int>("AppSettings:SmsCaptchaExpired", 300);
        await _distributedCacheClient.SetAsync(key, code, expiration ?? TimeSpan.FromSeconds(smsCaptchaExpired));

        var smsCaptchaSendExpired = _masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue<int>("AppSettings:SmsCaptchaSendExpired", 60);
        var smsSendExpiredKey = CacheKey.MsgVerifiyCodeSendExpired(key);
        await _distributedCacheClient.SetAsync(smsSendExpiredKey, smsSendExpiredKey, TimeSpan.FromSeconds(smsCaptchaSendExpired));

        return code;
    }

    public async Task<bool> VerifyMsgCodeAsync(string key, string code, bool removeCache = true)
    {
        var codeCache = await _distributedCacheClient.GetAsync<string>(key);
        if (codeCache != code) return false;
        if (removeCache)
        {
            await _distributedCacheClient.RemoveAsync<string>(key);
        }
        return true;
    }

    public async Task<bool> CheckAlreadySendAsync(string key)
    {
        var smsSendExpiredKey = CacheKey.MsgVerifiyCodeSendExpired(key);
        return await _distributedCacheClient.ExistsAsync<string>(smsSendExpiredKey);
    }
}
