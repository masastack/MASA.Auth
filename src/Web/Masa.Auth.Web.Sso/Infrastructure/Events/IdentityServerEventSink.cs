// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Events;

public class IdentityServerEventSink : IEventSink
{
    readonly ILogger<IdentityServerEventSink> _logger;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IUserSession _userSession;

    public IdentityServerEventSink(
        ILogger<IdentityServerEventSink> logger,
        IDistributedCacheClient distributedCacheClient,
        IUserSession userSession)
    {
        _logger = logger;
        _distributedCacheClient = distributedCacheClient;
        _userSession = userSession;
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
                if (evt.Id == EventIds.UserLoginSuccess)
                {
                    var userLoginSuccessEvent = evt as UserLoginSuccessEvent;
                    if (userLoginSuccessEvent != null)
                    {
                        var subjectId = userLoginSuccessEvent.SubjectId;
                        await _distributedCacheClient.SetAsync(subjectId, new LoginSession(subjectId, await _userSession.GetSessionIdAsync()));
                    }
                }
                else
                {
                    var userLogoutSuccessEvent = evt as UserLogoutSuccessEvent;
                    if (userLogoutSuccessEvent != null)
                    {
                        var key = string.Format(userLogoutSuccessEvent.SubjectId);
                        await _distributedCacheClient.RemoveAsync(key);
                    }
                }
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

internal class LoginSession
{
    public string Sub { get; set; }

    public string Sid { get; set; }

    public LoginSession(string sub, string sid)
    {
        Sub = sub;
        Sid = sid;
    }
}