// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Logout;

public partial class Index
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string LogoutId { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;
            if (User.Identity?.IsAuthenticated == true)
            {
                // if the user is not authenticated, then just show logged out page
                showLogoutPrompt = false;
            }
            else
            {
                var context = SsoAuthenticationStateCache.GetLogoutRequest(LogoutId);
                if (context?.ShowSignoutPrompt == false)
                {
                    // it's safe to automatically sign-out
                    showLogoutPrompt = false;
                }
            }
            if (!showLogoutPrompt)
            {
                Logout();
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private Task Logout()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed

            LogoutId ??= SsoAuthenticationStateCache.LogoutId;

            Navigation.NavigateTo($"logout?logoutId={LogoutId}", true);
        }
        else
        {
            Navigation.NavigateTo("/account/logout/loggedout", new Dictionary<string, object?> {
                {"logoutId", LogoutId}
            });
        }
    }
}
