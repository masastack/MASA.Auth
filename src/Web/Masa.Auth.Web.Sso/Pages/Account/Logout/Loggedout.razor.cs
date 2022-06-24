// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Logout;

public partial class Loggedout
{
    LoggedOutViewModel _viewModel = new();

    [Parameter]
    [SupplyParameterFromQuery]
    public string LogoutId { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(LogoutId);
            _viewModel.AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut;
            if (logout is not null)
            {
                _viewModel = new LoggedOutViewModel
                {
                    AutomaticRedirectAfterSignOut = LogoutOptions.AutomaticRedirectAfterSignOut,
                    PostLogoutRedirectUri = logout.PostLogoutRedirectUri,
                    ClientName = string.IsNullOrEmpty(logout.ClientName) ? logout.ClientId : logout.ClientName,
                    SignOutIframeUrl = logout.SignOutIFrameUrl
                };
                StateHasChanged();
            }

            if (_viewModel.AutomaticRedirectAfterSignOut)
            {
                await jsRuntime.InvokeAsync<IJSObjectReference>("import", "~/js/signout-redirect.js");
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
