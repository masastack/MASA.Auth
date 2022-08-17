// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Stores;

public class ConsentResponseStore : IConsentMessageStore
{
    readonly IDistributedCacheClient _distributedCacheClient;

    public ConsentResponseStore(IDistributedCacheClient distributedCacheClient)
    {
        _distributedCacheClient = distributedCacheClient;
    }

    public async Task DeleteAsync(string id)
    {
        await _distributedCacheClient.RemoveAsync<Message<ConsentResponse>>(id);
    }

    public async Task<Message<ConsentResponse>> ReadAsync(string id)
    {
        throw new NotImplementedException();
        //return await _distributedCacheClient.GetAsync<Message<ConsentResponse>>(id);
    }

    public async Task WriteAsync(string id, Message<ConsentResponse> message)
    {
        await _distributedCacheClient.SetAsync(id, message);
    }
}
