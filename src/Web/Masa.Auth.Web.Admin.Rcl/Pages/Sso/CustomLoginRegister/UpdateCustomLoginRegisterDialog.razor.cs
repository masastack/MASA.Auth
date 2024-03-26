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

    private string Tab { get; set; } = CustomLoginTab.BasicInformation;

    private MForm? Form { get; set; }

    protected override string? PageName { get; set; } = "CustomLoginBlock";

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
            Form.ResetValidation();
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

    public async Task UpdateCustomLoginAsync(FormContext context)
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
        if (!CustomLogin.RegisterFields.Any(r => (r.RegisterFieldType == RegisterFieldTypes.PhoneNumber || r.RegisterFieldType == RegisterFieldTypes.Email) && r.Required))
        {
            Tab = CustomLoginTab.Register;
            OpenErrorMessage(T("PhoneNumberAndEmailEmptyError"));
            return;
        }

        await CustomLoginService.UpdateAsync(CustomLogin);
        OpenSuccessMessage(T("Edit Custom Login data success"));
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
    }

    public void AddConfig()
    {
        if (Tab == CustomLoginTab.Login)
        {
            var maxSort = CustomLogin.ThirdPartyIdps.Select(tp => tp.Sort).DefaultIfEmpty().Max();
            CustomLogin.ThirdPartyIdps.Add(new(default, maxSort + 1));
        }
        else if (Tab == CustomLoginTab.Register)
        {
            var maxSort = CustomLogin.RegisterFields.Select(r => r.Sort).DefaultIfEmpty().Max();
            CustomLogin.RegisterFields.Add(new(default, maxSort + 1, default));
        }
    }
}
