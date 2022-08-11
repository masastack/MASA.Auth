// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Redirect;

[AllowAnonymous]
public partial class Index
{
    [Parameter]
    public string RedirectUri { get; set; } = string.Empty;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/signin-redirect.js");
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && !SsoUrlHelper.IsLocalUrl(RedirectUri))
        {
            Navigation.NavigateTo(GlobalVariables.ERROR_ROUTE, true);
        }
    }
}
