// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Others;

[AllowAnonymous]
public partial class Error
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string ErrorId { get; set; } = string.Empty;

    [Inject]
    public ILogger<Error> Logger { get; set; } = null!;

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

                // Error level so it is written even with the default "Error" log level;
                // use RequestId to correlate with the IdentityServer4 validation logs.
                Logger.LogError(
                    "IdentityServer error page: Error={Error}, ErrorDescription={ErrorDescription}, ClientId={ClientId}, RedirectUri={RedirectUri}, RequestId={RequestId}",
                    message.Error, message.ErrorDescription, message.ClientId, message.RedirectUri, message.RequestId);

                StateHasChanged();
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
