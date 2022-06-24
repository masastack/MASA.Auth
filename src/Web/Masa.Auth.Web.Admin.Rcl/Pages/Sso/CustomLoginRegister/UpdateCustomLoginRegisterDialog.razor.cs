// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister;

public partial class UpdateCustomLoginRegisterDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int CustomLoginId { get; set; }

    private CustomLoginDetailDto CustomLoginDetail { get; set; } = new();

    private UpdateCustomLoginDto CustomLogin { get; set; } = new();

    private CustomLoginService CustomLoginService => AuthCaller.CustomLoginService;

    private StringNumber Tab { get; set; } = CustomLoginTab.BasicInformation;

    private MForm? Form { get; set; }

    private ClientSelect? ClientSelectRef { get; set; }

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
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            Tab = CustomLoginTab.BasicInformation;
            await GetCustomLoginDetailAsync();
        }
    }

    public async Task GetCustomLoginDetailAsync()
    {
        CustomLoginDetail = await CustomLoginService.GetDetailAsync(CustomLoginId);
        CustomLogin = CustomLoginDetail;
    }

    public async Task UpdateCustomLoginAsync(EditContext context)
    {
        var success = context.Validate();
        if (success is false)
        {
            Tab = CustomLoginTab.BasicInformation;
            return;
        }
        if (CustomLogin.ThirdPartyIdps.Any(tp => tp.Id == default))
        {
            Tab = CustomLoginTab.Login;
            OpenErrorMessage(T("Login configuration items are required"));
            return;
        }
        if (CustomLogin.RegisterFields.Any(r => r.RegisterFieldType == default))
        {
            Tab = CustomLoginTab.Register;
            OpenErrorMessage(T("Register configuration items are required"));
            return;
        }

        Loading = true;
        await CustomLoginService.UpdateAsync(CustomLogin);
        OpenSuccessMessage("Update Custom Login success");
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
        Loading = false;
    }

    public void AddConfig()
    {
        if (Tab == CustomLoginTab.Login)
        {
            var maxSort = CustomLogin.ThirdPartyIdps.Count == 0 ? 0 : CustomLogin.ThirdPartyIdps.Max(tp => tp.Sort);
            CustomLogin.ThirdPartyIdps.Add(new(default, maxSort + 1));
        }
        else if (Tab == CustomLoginTab.Register)
        {
            var maxSort = CustomLogin.RegisterFields.Count == 0 ? 0 : CustomLogin.RegisterFields.Max(r => r.Sort);
            CustomLogin.RegisterFields.Add(new(default, maxSort + 1, default));
        }
    }
}
