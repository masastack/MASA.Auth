namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiScope;

public partial class AddApiScopeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddApiScopeDto ApiScope { get; set; } = new();

    private ApiScopeService ApiScopeService => AuthCaller.ApiScopeService;

    private MForm? Form { get; set; }

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
            ApiScope = new();
        }
    }

    public async Task AddApiScopeAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiScopeService.AddAsync(ApiScope);
            OpenSuccessMessage("Add apiScope success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
