// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Logout;

public partial class Index
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string LogoutId { get; set; } = string.Empty;

    bool _showLogoutPrompt;

    protected override async Task OnParametersSetAsync()
    {
        _showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;
        if (User.Identity?.IsAuthenticated != true)
        {
            // if user not authenticated, show logged out page
            _showLogoutPrompt = false;
        }
        else
        {
            var context = await _interaction.GetLogoutContextAsync(LogoutId);
            _showLogoutPrompt = context?.ShowSignoutPrompt == true;
        }
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_showLogoutPrompt)
        {
            Logout();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void Logout()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Navigation.NavigateTo($"logout?logoutId={LogoutId}", true);
        }
        else
        {
            Navigation.NavigateTo($"/account/logout/loggedout?logoutId={LogoutId}", true);
        }
    }
}
