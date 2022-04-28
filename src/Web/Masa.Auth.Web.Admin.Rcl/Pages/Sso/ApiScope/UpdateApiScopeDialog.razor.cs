namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiScope;

public partial class UpdateApiScopeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int ApiScopeId { get; set; }

    private ApiScopeDetailDto ApiScopeDetail { get; set; } = new();

    private UpdateApiScopeDto ApiScope { get; set; } = new();

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetApiScopeDetailAsync();
        }
    }

    public async Task GetApiScopeDetailAsync()
    {
        ApiScopeDetail = await ApiScopeService.GetDetailAsync(ApiScopeId);
        ApiScope = ApiScopeDetail;
    }

    public async Task UpdatetApiScopeAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiScopeService.UpdateAsync(ApiScope);
            OpenSuccessMessage("Update apiScope success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
