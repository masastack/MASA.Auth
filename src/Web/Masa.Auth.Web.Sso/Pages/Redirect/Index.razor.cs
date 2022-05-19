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

    [Inject]
    public IUrlHelper Url { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && !Url.IsLocalUrl(RedirectUri))
        {
            Navigation.NavigateTo(GlobalVariables.ERROR_ROUTE,true);
        }
    }
}
