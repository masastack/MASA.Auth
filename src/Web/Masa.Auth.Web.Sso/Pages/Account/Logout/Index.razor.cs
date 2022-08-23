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
        // Simplify flow ignore judge ShowLogoutPrompt 
        //var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        //if (User?.Identity?.IsAuthenticated != true)
        //{
        //    // if the user is not authenticated, then just show logged out page
        //    showLogoutPrompt = false;
        //}
        //else
        //{
        //    var context = await _interaction.GetLogoutContextAsync(LogoutId);
        //    showLogoutPrompt = context?.ShowSignoutPrompt == true;
        //}

        //if (firstRender && showLogoutPrompt == false)
        //{
        //    Logout();
        //    return;
        //}
        await base.OnAfterRenderAsync(firstRender);
    }

    private void Logout()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Navigation.NavigateTo($"logout", new Dictionary<string, object?>
            {
                 { "logoutId",LogoutId },
            }, true);
        }
        else
        {
            Navigation.NavigateTo($"/account/logout/loggedout", new Dictionary<string, object?>
            {
                 { "logoutId",LogoutId },
            });
        }
    }
}
