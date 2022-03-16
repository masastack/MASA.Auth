using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddOrEditStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid StaffId { get; set; }

    private bool IsAdd => StaffId == Guid.Empty;

    private StaffDetailResponse Staff { get; set; } = StaffDetailResponse.Default;

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
            if (IsAdd) Staff = StaffDetailResponse.Default;
            else await GetStaffDetailAsync();
        }
    }

    public async Task GetStaffDetailAsync()
    {
        var response = await AuthClient.GetStaffDetailAsync(StaffId);
        if (response.Success)
        {
            Staff = response.Data;
        }
        else OpenErrorDialog($"Failed to query staff data:{response.Message}");
    }

    public async Task AddOrEditStaffAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var reponse = await AuthClient.AddStaffAsync(Staff);
            if (reponse.Success)
            {
                OpenSuccessMessage(T("Add staff success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog($"Failed to add staff:{reponse.Message}");
        }
        else
        {
            var reponse = await AuthClient.EditStaffAsync(Staff);
            if (reponse.Success)
            {
                OpenSuccessMessage("Edit staff success");
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog($"Failed to edit staff:{reponse.Message}");
        }
        Loading = false;
    }

    public void OpenDeleteStaffDialog()
    {
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await DeleteStaffAsync();
        }, T("Are you sure delete staff data"));
    }

    public async Task DeleteStaffAsync()
    {
        Loading = true;
        var response = await AuthClient.DeleteStaffAsync(StaffId);
        if (response.Success) OpenSuccessMessage(T("Delete staff data success"));
        else OpenErrorDialog(T("Delete staff data failed:") + response.Message);
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

