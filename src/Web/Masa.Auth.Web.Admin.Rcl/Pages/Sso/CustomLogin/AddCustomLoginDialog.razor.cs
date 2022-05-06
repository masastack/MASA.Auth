namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class AddCustomLoginDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddCustomLoginDto CustomLogin { get; set; } = new();

    private CustomLoginService CustomLoginService => AuthCaller.CustomLoginService;

    private StringNumber Tab { get; set; } = CustomLoginTab.BasicInformation;

    private MForm? Form { get; set; }

    private ClientSelect? Client { get; set; }

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

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            CustomLogin = new();
        }
    }

    public async Task AddCustomLoginAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await CustomLoginService.AddAsync(CustomLogin);
            OpenSuccessMessage("Add customLogin success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
