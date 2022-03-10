namespace Masa.Auth.Admin.Pages.Subjects.ThirdPartyPlatforms;

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
        if(Visible is true)
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
        else OpenErrorMessage("Failed to query thirdPartyPlatform data !");
    }

    public async Task AddOrEditThirdPartyPlatformAsync()
    {
        Lodding = true;
        if(IsAdd)
        {
            var reponse = await AuthClient.AddThirdPartyPlatformAsync(ThirdPartyPlatform);
            if (reponse.Success)
            {
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog("Failed to add thirdPartyPlatform !");
        }
        else
        {
            var reponse = await AuthClient.EditThirdPartyPlatformAsync(ThirdPartyPlatform);
            if (reponse.Success)
            {
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog("Failed to edit thirdPartyPlatform !");
        }
        Lodding = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

