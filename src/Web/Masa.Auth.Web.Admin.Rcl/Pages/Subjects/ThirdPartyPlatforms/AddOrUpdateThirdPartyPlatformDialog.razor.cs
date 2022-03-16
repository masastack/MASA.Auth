namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class AddOrEditThirdPartyIdpDialog
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

    private ThirdPartyIdpItemResponse ThirdPartyIdp { get; set; } = ThirdPartyIdpItemResponse.Default;

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
        if (Visible is true)
        {
            if (IsAdd) ThirdPartyIdp = ThirdPartyIdpItemResponse.Default;
            else await GetThirdPartyIdpDetailAsync();
        }
    }

    public async Task GetThirdPartyIdpDetailAsync()
    {
        var response = await AuthClient.GetThirdPartyIdpDetailAsync(ThirdPartyIdpId);
        if (response.Success)
        {
            ThirdPartyIdp = response.Data;
        }
        else OpenErrorMessage(T("Failed to add thirdPartyIdpDetail data:") + response.Message);
    }

    public async Task AddOrEditThirdPartyIdpAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var response = await AuthClient.AddThirdPartyIdpAsync(ThirdPartyIdp);
            if (response.Success)
            {
                OpenSuccessMessage(T("Add thirdPartyIdp success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to add thirdPartyIdp:") + response.Message);
        }
        else
        {
            var response = await AuthClient.EditThirdPartyIdpAsync(ThirdPartyIdp);
            if (response.Success)
            {
                OpenSuccessMessage(T("Edit thirdPartyIdp success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to edit thirdPartyIdp:") + response.Message);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

