// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Redirect;

public class RedirectToLogin : ComponentBase
{
    [Inject]
    protected NavigationManager Navigation { get; set; } = null!;

    [Parameter]
    public string ReturnUrl { get; set; } = string.Empty;

    [Parameter]
    public IReadOnlyDictionary<string, object> Data { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            var url = "account/login";
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                url += $"?ReturnUrl={ReturnUrl}";
            }
            Navigation.NavigateTo(url, true);
        }
        base.OnAfterRender(firstRender);
    }
}
