using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ViewThirdPartyUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public Guid ThirdPartyUserId { get; set; }

    private ThirdPartyUserItemResponse ThirdPartyUser { get; set; } = ThirdPartyUserItemResponse.Default;

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
            var response = await AuthClient.GetThirdPartyUserDetailAsync(ThirdPartyUserId);
            if (response.Success)
            {
                ThirdPartyUser = response.Data;
            }
            else OpenErrorDialog(T("Failed to query thirdPartyUserDetail data:") + response.Message);
        }
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

