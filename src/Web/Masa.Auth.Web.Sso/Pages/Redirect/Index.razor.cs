// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Redirect;

[AllowAnonymous]
public partial class Index
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// 取多语言词条。词条由 Lingua 平台供给，平台尚未发布新词条时 T() 会原样返回 key，
    /// 此时回退到内置文案，避免页面直接显示 key。
    /// </summary>
    private string Text(string key, string zh, string en)
    {
        var text = T(key);
        if (!string.IsNullOrEmpty(text) && text != key)
        {
            return text;
        }

        return LanguageProvider.Culture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ? zh : en;
    }

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
