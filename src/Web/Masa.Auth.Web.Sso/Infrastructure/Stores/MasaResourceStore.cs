// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IMasaOidcResourceStore = Masa.BuildingBlocks.Authentication.Oidc.Storage.Stores.IResourceStore;

namespace Masa.Auth.Web.Sso.Infrastructure.Stores;

public class MasaResourceStore : IResourceStore
{
    readonly IMasaOidcResourceStore _masaOidcResourceStore;

    public MasaResourceStore(IMasaOidcResourceStore masaOidcResourceStore)
    {
        _masaOidcResourceStore = masaOidcResourceStore;
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        return (await _masaOidcResourceStore.FindApiResourcesByNameAsync(apiResourceNames)).Adapt<IEnumerable<ApiResource>>();
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        return (await _masaOidcResourceStore.FindApiResourcesByScopeNameAsync(scopeNames)).Adapt<IEnumerable<ApiResource>>();
    }

    public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        return (await _masaOidcResourceStore.FindApiScopesByNameAsync(scopeNames)).Adapt<IEnumerable<ApiScope>>();
    }

    public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        return (await _masaOidcResourceStore.FindIdentityResourcesByScopeNameAsync(scopeNames)).Adapt<IEnumerable<IdentityResource>>();
    }

    public async Task<Resources> GetAllResourcesAsync()
    {
        return (await _masaOidcResourceStore.GetAllResourcesAsync()).Adapt<Resources>();
    }
}
