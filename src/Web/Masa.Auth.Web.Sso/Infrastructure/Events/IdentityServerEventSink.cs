// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Events;

public class IdentityServerEventSink : IEventSink
{
    const string OnlineUsersKey = "online:users";
    const string SsoSessionKeyPrefix = "online:sso-session:";

    static string SsoSessionKey(string subjectId) => $"{SsoSessionKeyPrefix}{subjectId}";

    readonly ILogger<IdentityServerEventSink> _logger;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IUserSession _userSession;
    readonly IConnectionMultiplexer _mux;

    public IdentityServerEventSink(
        ILogger<IdentityServerEventSink> logger,
        IDistributedCacheClient distributedCacheClient,
        IUserSession userSession,
        IConnectionMultiplexer mux)
    {
        _logger = logger;
        _distributedCacheClient = distributedCacheClient;
        _userSession = userSession;
        _mux = mux;
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

                        var db = _mux.GetDatabase();
                        await db.SetAddAsync(OnlineUsersKey, subjectId);
                        await db.KeyDeleteAsync($"kicked:{subjectId}");
                        var sessionInfo = new WebOnlineSession(DateTime.UtcNow, userLoginSuccessEvent.ClientId);
                        await db.StringSetAsync(SsoSessionKey(subjectId), JsonSerializer.Serialize(sessionInfo), TimeSpan.FromHours(24));
                    }
                }
                else
                {
                    var userLogoutSuccessEvent = evt as UserLogoutSuccessEvent;
                    if (userLogoutSuccessEvent != null)
                    {
                        var subjectId = userLogoutSuccessEvent.SubjectId;
                        await _distributedCacheClient.RemoveAsync(subjectId);

                        var db = _mux.GetDatabase();
                        await db.SetRemoveAsync(OnlineUsersKey, subjectId);
                        await db.KeyDeleteAsync(SsoSessionKey(subjectId));
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

internal record WebOnlineSession(DateTime LoginTime, string? ClientId);