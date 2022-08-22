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
        var vm = await BuildLogoutViewModelAsync(LogoutId);
        if (firstRender && vm.ShowLogoutPrompt == false)
        {
            Logout();
            return;
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

    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = LogoutOptions.ShowLogoutPrompt };

        if (User?.Identity?.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        var context = await _interaction.GetLogoutContextAsync(LogoutId);
        vm.ShowLogoutPrompt = context?.ShowSignoutPrompt == true;

        // show the logout prompt. this prevents attacks where the user
        // is automatically signed out by another malicious web page.
        return vm;
    }
}
