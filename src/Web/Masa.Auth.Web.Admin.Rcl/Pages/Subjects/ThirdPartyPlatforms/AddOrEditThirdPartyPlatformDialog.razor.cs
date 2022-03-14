namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyPlatforms;

public partial class AddOrEditThirdPartyPlatformDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ThirdPartyPlatformId { get; set; }

    private bool IsAdd => ThirdPartyPlatformId == Guid.Empty;

    private ThirdPartyPlatformItemResponse ThirdPartyPlatform { get; set; } = ThirdPartyPlatformItemResponse.Default;

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
            if (IsAdd) ThirdPartyPlatform = ThirdPartyPlatformItemResponse.Default;
            else await GetThirdPartyPlatformDetailAsync();
        }
    }

    public async Task GetThirdPartyPlatformDetailAsync()
    {
        var response = await AuthClient.GetThirdPartyPlatformDetailAsync(ThirdPartyPlatformId);
        if (response.Success)
        {
            ThirdPartyPlatform = response.Data;
        }
        else OpenErrorMessage(T("Failed to add thirdPartyPlatformDetail data:") + response.Message);
    }

    public async Task AddOrEditThirdPartyPlatformAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var response = await AuthClient.AddThirdPartyPlatformAsync(ThirdPartyPlatform);
            if (response.Success)
            {
                OpenSuccessMessage(T("Add thirdPartyPlatform success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to add thirdPartyPlatform:") + response.Message);
        }
        else
        {
            var response = await AuthClient.EditThirdPartyPlatformAsync(ThirdPartyPlatform);
            if (response.Success)
            {
                OpenSuccessMessage(T("Edit thirdPartyPlatform success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to edit thirdPartyPlatform:") + response.Message);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

