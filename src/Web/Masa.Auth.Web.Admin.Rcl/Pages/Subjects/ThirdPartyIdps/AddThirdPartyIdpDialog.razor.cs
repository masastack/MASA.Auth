// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class AddThirdPartyIdpDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyIdpId { get; set; }

    private AddThirdPartyIdpDto ThirdPartyIdp { get; set; } = new();

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override string? PageName { get; set; } = "ThirdPartyIdpBlock";

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            ThirdPartyIdp = new();
        }
    }

    public async Task AddThirdPartyIdpAsync(EditContext context)
    {
        if (string.IsNullOrEmpty(ThirdPartyIdp.Icon))
        {
            OpenErrorMessage(T("Please upload ThirdPartyIdp icon"));
            return;
        }
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ThirdPartyIdpService.AddAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Add thirdPartyIdp success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    void ThirdPartyIdpValueChanged(AuthenticationDefaults value)
    {
        ThirdPartyIdp.ThirdPartyIdpType = Enum.Parse<ThirdPartyIdpTypes>(value.Scheme);
        ThirdPartyIdp.Icon = value.Icon;
        ThirdPartyIdp.Name = value.Scheme;
        ThirdPartyIdp.DisplayName = value.DisplayName;
        ThirdPartyIdp.CallbackPath = value.CallbackPath;
        ThirdPartyIdp.AuthorizationEndpoint = value.AuthorizationEndpoint;
        ThirdPartyIdp.TokenEndpoint = value.TokenEndpoint;
        ThirdPartyIdp.UserInformationEndpoint = value.UserInformationEndpoint;
    }
}

