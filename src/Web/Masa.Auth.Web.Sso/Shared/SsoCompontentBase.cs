// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Shared;

public abstract class SsoCompontentBase : SsoSectionComponentBase
{
    [Inject]
    public IIdentityServerInteractionService Interaction { get; set; } = null!;

    [Inject]
    public IEventService Events { get; set; } = null!;

    [Inject]
    public AuthenticationStateProvider _authenticationStateProvider { get; set; } = null!;

    protected ClaimsPrincipal User
    {
        get
        {
            return _authenticationStateProvider.GetAuthenticationStateAsync().Result.User;
        }
    }
}