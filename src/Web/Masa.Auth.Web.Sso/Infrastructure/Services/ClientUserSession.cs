// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Services;

/// <summary>
/// Properties Items lost client_list key,do not know why, use redis replace.
/// </summary>
public class ClientUserSession : DefaultUserSession
{
    readonly IDistributedCacheClient _distributedCacheClient;

    public ClientUserSession(
        IHttpContextAccessor httpContextAccessor,
        IAuthenticationHandlerProvider handlers,
        IdentityServerOptions options,
        ISystemClock clock,
        ILogger<IUserSession> logger,
        IDistributedCacheClient distributedCacheClient) : base(httpContextAccessor, handlers, options, clock, logger)
    {
        _distributedCacheClient = distributedCacheClient;
    }

    public override async Task<IEnumerable<string>> GetClientListAsync()
    {
        var clientList = (await base.GetClientListAsync()).ToList();
        var backUpClientList = await _distributedCacheClient.GetAsync<List<string>>(await GetSessionIdAsync());
        if (!clientList.Any() && backUpClientList != null)
        {
            clientList = backUpClientList;
        }
        return clientList;
    }

    public override async Task AddClientIdAsync(string clientId)
    {
        var clientIds = (await GetClientListAsync()).ToList();
        if (!clientIds.Contains(clientId))
        {
            clientIds.Add(clientId);
            await _distributedCacheClient.SetAsync(await GetSessionIdAsync(), clientIds, Properties.ExpiresUtc ?? DateTimeOffset.Now.AddDays(30));
            await base.AddClientIdAsync(clientId);
        }
    }

    public override async Task RemoveSessionIdCookieAsync()
    {
        var sessionId = await GetSessionIdAsync();
        if (sessionId != null)
        {
            await _distributedCacheClient.RemoveAsync(sessionId);
        }
        await base.RemoveSessionIdCookieAsync();
    }
}
