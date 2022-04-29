namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiResource;

public partial class AddApiResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddApiResourceDto ApiResource { get; set; } = new();

    private ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

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
            ApiResource = new();
        }
    }

    public async Task AddApiResourceAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiResourceService.AddAsync(ApiResource);
            OpenSuccessMessage("Add apiScope success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
