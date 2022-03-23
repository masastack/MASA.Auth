namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class AddOrUpdateThirdPartyIdpDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyIdpId { get; set; }

    private bool IsAdd => ThirdPartyIdpId == Guid.Empty;

    private ThirdPartyIdpDetailDto ThirdPartyIdp { get; set; } = new();

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            if (IsAdd) ThirdPartyIdp = new();
            else await GetThirdPartyIdpDetailAsync();
        }
    }

    public async Task GetThirdPartyIdpDetailAsync()
    {
        ThirdPartyIdp = await ThirdPartyIdpService.GetThirdPartyIdpDetailAsync(ThirdPartyIdpId);
    }

    public async Task AddOrEditThirdPartyIdpAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            await ThirdPartyIdpService.AddThirdPartyIdpAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Add thirdPartyIdp success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        else
        {
            await ThirdPartyIdpService.UpdateThirdPartyIdpAsync(ThirdPartyIdp);
            OpenSuccessMessage(T("Edit thirdPartyIdp success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

