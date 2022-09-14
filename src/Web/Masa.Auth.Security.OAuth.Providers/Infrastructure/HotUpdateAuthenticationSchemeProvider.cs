// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Authentication;

public class HotUpdateAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    IAuthenticationSchemeInUseProvider _authenticationSchemeInUseProvider;

    public HotUpdateAuthenticationSchemeProvider(
        IOptions<AuthenticationOptions> options,
        IAuthenticationSchemeInUseProvider authenticationSchemeInUseProvider) : base(options)
    {
        _authenticationSchemeInUseProvider = authenticationSchemeInUseProvider;
    }

    public override async Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        var authenticationScheme = await base.GetSchemeAsync(name);
        if(authenticationScheme is null)
        {
            var scheme = _authenticationSchemeInUseProvider.GetAllSchemes().FirstOrDefault(scheme => scheme == name);
            if (scheme is null)
            {
                return null;
            }
            else return LocalAuthenticationSchemeProvider.GetScheme(scheme);
        }
        else return authenticationScheme;
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
    {
        var authenticationSchemes = await base.GetRequestHandlerSchemesAsync();
        var schemes = _authenticationSchemeInUseProvider.GetAllSchemes();
        return LocalAuthenticationSchemeProvider.GetSchemes(schemes)
                                                .Concat(authenticationSchemes);
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
    {
        var authenticationSchemes = await base.GetAllSchemesAsync();
        var schemes = _authenticationSchemeInUseProvider.GetAllSchemes();
        return LocalAuthenticationSchemeProvider.GetSchemes(schemes)
                                                .Concat(authenticationSchemes);
    }
}
