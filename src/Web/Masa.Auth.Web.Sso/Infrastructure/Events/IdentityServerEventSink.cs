// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Events;

public class IdentityServerEventSink : IEventSink
{
    readonly ILogger<IdentityServerEventSink> _logger;
    readonly IDistributedCacheClient _distributedCacheClient;

    public IdentityServerEventSink(ILogger<IdentityServerEventSink> logger, IDistributedCacheClient distributedCacheClient)
    {
        _logger = logger;
        _distributedCacheClient = distributedCacheClient;
    }

    public async Task PersistAsync(Event evt)
    {

        if (evt.EventType == EventTypes.Success ||
            evt.EventType == EventTypes.Information)
        {
            _logger.LogInformation("{Name} ({Id}), Details: {@details}",
                evt.Name,
                evt.Id,
                evt);

            if (evt.Id == EventIds.UserLoginSuccess || evt.Id == EventIds.UserLogoutSuccess)
            {
                var loginUsers = await _distributedCacheClient.GetAsync<HashSet<string>>(GlobalVariables.LOGIN_USER_CACHE_KEY) ?? new();
                if (evt.Id == EventIds.UserLoginSuccess)
                {
                    var userLoginSuccessEvent = evt as UserLoginSuccessEvent;
                    if (userLoginSuccessEvent != null)
                    {
                        loginUsers.Add(userLoginSuccessEvent.SubjectId);
                    }
                }
                else
                {
                    var userLogoutSuccessEvent = evt as UserLogoutSuccessEvent;
                    if (userLogoutSuccessEvent != null)
                    {
                        loginUsers.Remove(userLogoutSuccessEvent.SubjectId);
                    }
                }
                await _distributedCacheClient.SetAsync(GlobalVariables.LOGIN_USER_CACHE_KEY, loginUsers);
            }
        }
        else
        {
            _logger.LogError("{Name} ({Id}), Details: {@details}",
                evt.Name,
                evt.Id,
                evt);
        }
    }
}
