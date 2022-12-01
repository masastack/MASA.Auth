﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class SsoAuthenticationStateCache
{
    private ConcurrentDictionary<string, AuthorizationRequest> AuthorizationRequestCache
            = new ConcurrentDictionary<string, AuthorizationRequest>();

    public IEnumerable<Grant> Grants { get; set; } = new List<Grant>();

    public bool HasAuthorizationRequest(string url) => AuthorizationRequestCache.ContainsKey(url);

    public AuthorizationRequest? GetAuthorizationContext(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }
        AuthorizationRequestCache.TryGetValue(url, out var data);
        return data;
    }

    public void RemoveAuthorizationContext(string url)
    {
        Debug.WriteLine($"Removing url: {url}");
        AuthorizationRequestCache.TryRemove(url, out _);
    }

    public void AddAuthorizationContext(string url, AuthorizationRequest data)
    {
        Debug.WriteLine($"Adding url: {url}");
        if (HasAuthorizationRequest(url))
        {
            AuthorizationRequestCache[url] = data;
        }
        else
        {
            AuthorizationRequestCache.TryAdd(url, data);
        }
    }
}
