// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Email;

public class EmailAgent : IScopedDependency
{
    readonly IMcClient _mcClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IOptions<EmailOptions> _emailOptions;

    public EmailAgent(IMcClient mcClient, IDistributedCacheClient distributedCacheClient, IOptions<EmailOptions> emailOptions)
    {
        _mcClient = mcClient;
        _distributedCacheClient = distributedCacheClient;
        _emailOptions = emailOptions;
    }

    public async Task SendEmailAsync(SendEmailModel sendEmailModel, TimeSpan? expiration = null)
    {
        //todo Abstract Factory
        var sendKey = CacheKey.EmailCodeRegisterSendKey(sendEmailModel.Email);
        if (await CheckAlreadySendAsync(sendKey))
        {
            throw new UserFriendlyException("Email has been sent, please try again later");
        }
        var code = Random.Shared.Next(100000, 999999).ToString();
        await _mcClient.MessageTaskService.SendTemplateMessageAsync(new SendTemplateMessageModel
        {
            ChannelCode = _emailOptions.Value.ChannelCode,
            ChannelType = ChannelTypes.Email,
            TemplateCode = _emailOptions.Value.TemplateCode,
            ReceiverType = SendTargets.Assign,
            Receivers = new List<MessageTaskReceiverModel>
            {
                new MessageTaskReceiverModel
                {
                    Type = MessageTaskReceiverTypes.User,
                    Email = sendEmailModel.Email
                }
            },
            Variables = new ExtraPropertyDictionary(new Dictionary<string, object>
            {
                ["code"] = code,
            })
        });

        await _distributedCacheClient.SetAsync(CacheKey.EmailCodeRegisterKey(sendEmailModel.Email), code, expiration ?? TimeSpan.FromMinutes(5));
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
