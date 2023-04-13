namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class SGeneratePassword : STextField<string>
{
    [Parameter]
    public bool DisableConfirmDialog { get; set; } = false;

    [Inject]
    public PasswordHelper PasswordHelper { get; set; } = null!;

    private bool ConfirmDialogVisible { get; set; } = false;

    public new string Type { get; set; } = "password";

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = I18n!.T("Password");
        Readonly = true;
        base.Type = Type;
        AppendContent = builder =>
        {
            if (Type == "text")
            {
                builder.OpenComponent<PCopyableText>(0);
                builder.AddAttribute(1, "Class", "ml-n9");
                builder.AddAttribute(2, "Text", Value);
                builder.CloseComponent();
            }
        };
        return base.SetParametersAsync(parameters);
    }

    public async Task OnResetPasswordAsync()
    {
        if (!DisableConfirmDialog)
        {
            ConfirmDialogVisible = true;
            return;
        }

        await ResetPasswordAsync();
    }

    public async Task ResetPasswordAsync()
    {
        Type = "text";
        var value = PasswordHelper.GenerateNewPassword();
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
