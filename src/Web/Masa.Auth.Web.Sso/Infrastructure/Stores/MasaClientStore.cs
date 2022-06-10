// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IMasaOidcClientStore = Masa.BuildingBlocks.Oidc.Storage.Stores.IClientStore;

namespace Masa.Auth.Web.Sso.Infrastructure.Stores;

public class MasaClientStore : IClientStore
{
    readonly IMasaOidcClientStore _masaOidcClientStore;
    public MasaClientStore(IMasaOidcClientStore masaOidcClientStore)
    {
        _masaOidcClientStore = masaOidcClientStore;
    }
    public async Task<Client> FindClientByIdAsync(string clientId)
    {
        return (await _masaOidcClientStore.FindClientByIdAsync(clientId))?.Adapt<Client>() ?? new();
    }
}
