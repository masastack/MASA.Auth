// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class PropertiesDataFormatCache : ISecureDataFormat<AuthenticationProperties>
{
    readonly IDistributedCacheClient _distributedCacheClient;

    public PropertiesDataFormatCache(IDistributedCacheClient distributedCacheClient)
    {
        _distributedCacheClient = distributedCacheClient;
    }

    public string Protect(AuthenticationProperties data)
    {
        var key = CreatKey();
        _distributedCacheClient.Set(key, data, TimeSpan.FromSeconds(60));
        return key;
    }

    public string Protect(AuthenticationProperties data, string? purpose)
    {
        throw new NotImplementedException();
    }

    public AuthenticationProperties? Unprotect(string? protectedText)
    {
        if (protectedText is null) return null;
        return _distributedCacheClient.Get<AuthenticationProperties>(protectedText);
    }

    public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
    {
        throw new NotImplementedException();
    }

    string CreatKey()
    {
        return $"oauth_authentication_properties_{Guid.NewGuid()}";
    }
}
