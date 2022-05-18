// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Others;

[AllowAnonymous]
[SecurityHeaders]
public partial class Error
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string ErrorId { get; set; } = string.Empty;

    ErrorViewModel _errorViewModel = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // retrieve error details from identityserver
            var message = await Interaction.GetErrorContextAsync(ErrorId);
            if (message != null)
            {
                _errorViewModel.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
                StateHasChanged();
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
