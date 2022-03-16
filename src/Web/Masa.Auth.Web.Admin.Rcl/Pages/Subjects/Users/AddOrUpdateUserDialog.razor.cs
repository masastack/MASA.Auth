using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddOrEditUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    private bool IsAdd => UserId == Guid.Empty;

    private UserItemResponse User { get; set; } = UserItemResponse.Default;

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
            if (IsAdd) User = UserItemResponse.Default;
            else await GetUserDetailAsync();
        }
    }

    public async Task GetUserDetailAsync()
    {
        var response = await AuthClient.GetUserDetailAsync(UserId);
        if (response.Success)
        {
            User = response.Data;
        }
        else OpenErrorMessage(T("Failed to query staffDetail data:") + response.Message);
    }

    public async Task AddOrEditUserAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var response = await AuthClient.AddUserAsync(User);
            if (response.Success)
            {
                OpenSuccessMessage(T("Add staff data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to add staff:") + response.Message);
        }
        else
        {
            var response = await AuthClient.EditUserAsync(User);
            if (response.Success)
            {
                OpenSuccessMessage(T("Edit staff data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to edit staff:") + response.Message);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

