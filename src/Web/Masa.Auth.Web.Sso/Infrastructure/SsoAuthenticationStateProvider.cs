// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class SsoAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    public SsoAuthenticationStateProvider(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(10);

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        var d = authenticationState.User.IsAuthenticated();
        var sid =
            authenticationState.User.Claims
            .Where(c => c.Type.Equals("sid"))
            .Select(c => c.Value)
            .FirstOrDefault();

        var name =
            authenticationState.User.Claims
            .Where(c => c.Type.Equals("name"))
            .Select(c => c.Value)
            .FirstOrDefault() ?? string.Empty;
        Debug.WriteLine($"\nValidate: {name} / {sid}");

        return Task.FromResult(true);
    }
}
