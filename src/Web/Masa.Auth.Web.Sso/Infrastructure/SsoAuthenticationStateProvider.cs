// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class SsoAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    readonly ILogger _logger;

    public SsoAuthenticationStateProvider(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SsoAuthenticationStateProvider>();
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
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

        _logger.LogInformation($"\nValidate: {name} / {sid}");

        return Task.FromResult(true);
    }
}
