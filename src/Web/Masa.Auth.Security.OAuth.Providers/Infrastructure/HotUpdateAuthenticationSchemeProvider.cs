// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Authentication;

public class HotUpdateAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    readonly IRemoteAuthenticationDefaultsProvider _remoteAuthenticationDefaultsProvider;

    public HotUpdateAuthenticationSchemeProvider(
        IOptions<AuthenticationOptions> options,
        IRemoteAuthenticationDefaultsProvider remoteAuthenticationDefaultsProvider) : base(options)
    {
        _remoteAuthenticationDefaultsProvider = remoteAuthenticationDefaultsProvider;
    }

    public override async Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        var authenticationScheme = await base.GetSchemeAsync(name);
        if(authenticationScheme is null)
        {
            var authenticationDefaults = await _remoteAuthenticationDefaultsProvider.GetAsync(name);
            if (authenticationDefaults is null)
            {
                return null;
            }
            else return LocalAuthenticationSchemeProvider.GetScheme(authenticationDefaults.Scheme);
        }
        else return authenticationScheme;
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
    {
        var authenticationSchemes = await base.GetRequestHandlerSchemesAsync();
        var authenticationDefaults = await _remoteAuthenticationDefaultsProvider.GetAllAsync();
        return LocalAuthenticationSchemeProvider.GetSchemes(authenticationDefaults.Select(item => item.Scheme))
                                                .Concat(authenticationSchemes);
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
    {
        var authenticationSchemes = await base.GetAllSchemesAsync();
        var authenticationDefaults = await _remoteAuthenticationDefaultsProvider.GetAllAsync();
        return LocalAuthenticationSchemeProvider.GetSchemes(authenticationDefaults.Select(item => item.Scheme))
                                                .Concat(authenticationSchemes);
    }
}
