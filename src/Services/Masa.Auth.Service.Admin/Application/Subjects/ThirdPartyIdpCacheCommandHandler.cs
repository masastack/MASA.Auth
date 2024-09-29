// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class ThirdPartyIdpCacheCommandHandler
{
    readonly IDistributedCacheClient _cacheClient;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;

    public ThirdPartyIdpCacheCommandHandler(IDistributedCacheClient cacheClient, IThirdPartyIdpRepository thirdPartyIdpRepository)
    {
        _cacheClient = cacheClient;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
    }

    [EventHandler(99)]
    public async Task SyncCacheAsync(AddThirdPartyIdpCommand command)
    {
        await SyncCacheAsync();
    }

    [EventHandler(99)]
    public async Task SyncCacheAsync(UpdateThirdPartyIdpCommand command)
    {
        await SyncCacheAsync();
    }

    [EventHandler(99)]
    public async Task SyncCacheAsync(RemoveThirdPartyIdpCommand command)
    {
        await SyncCacheAsync();
    }

    async Task SyncCacheAsync()
    {
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync(tpIdp => tpIdp.Enabled);
        await _cacheClient.SetAsync(CacheKeyConsts.ALL_THIRD_PARTY_IDP, thirdPartyIdps.Adapt<List<ThirdPartyIdpModel>>());
    }
}
