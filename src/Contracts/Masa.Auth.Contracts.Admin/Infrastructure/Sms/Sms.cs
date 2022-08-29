// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Sms;

public class Sms : IScopedDependency
{
    readonly IMcClient _mcClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IOptions<SmsOptions> _smsOptions;

    public Sms(IMcClient mcClient, IDistributedCacheClient distributedCacheClient, IOptions<SmsOptions> smsOptions)
    {
        _mcClient = mcClient;
        _distributedCacheClient = distributedCacheClient;
        _smsOptions = smsOptions;
    }

    public async Task<string> SendMsgCodeAsync(string key, string phoneNumber, TimeSpan? expiration = null)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        await _mcClient.MessageTaskService.SendTemplateMessageAsync(new SendTemplateMessageModel
        {
            ChannelCode = _smsOptions.Value.ChannelCode,
            ChannelType = ChannelTypes.Sms,
            TemplateCode = _smsOptions.Value.TemplateCode,
            ReceiverType = SendTargets.Assign,
            Receivers = new List<MessageTaskReceiverModel>
            {
                new MessageTaskReceiverModel
                {
                    Type = MessageTaskReceiverTypes.User,
                    PhoneNumber = phoneNumber
                }
            },
            Variables = new ExtraPropertyDictionary(new Dictionary<string, object>
            {
                ["code"] = code,
            })
        });

        var options = new CombinedCacheEntryOptions<string>
        {
            DistributedCacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(1)
            }
        };
        await _distributedCacheClient.SetAsync(key, code, options);

        return code;
    }

    public async Task<bool> VerifyMsgCodeAsync(string key, string code)
    {
        var codeCache = await _distributedCacheClient.GetAsync<string>(key);
        if (codeCache != code) return false;
        await _distributedCacheClient.RemoveAsync<string>(key);
        return true;
    }

    public async Task<bool> CheckAlreadySendAsync(string key)
    {
        return await _distributedCacheClient.ExistsAsync<string>(key);
    }
}
