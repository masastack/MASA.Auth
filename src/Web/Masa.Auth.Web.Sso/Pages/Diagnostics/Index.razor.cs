// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Diagnostics;

[SecurityHeaders]
[Authorize]
public partial class Index
{
    ViewModel? _viewModel;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _httpContextAccessor.HttpContext != null)
        {
            var localAddresses = new string[] { "127.0.0.1", "::1", _httpContextAccessor.HttpContext.Connection.LocalIpAddress?.ToString() ?? "" };
            if (!localAddresses.Contains(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString()))
            {
                Navigation.NavigateTo("404");
                return;
            }

            _viewModel = new ViewModel(await _httpContextAccessor.HttpContext.AuthenticateAsync());
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
