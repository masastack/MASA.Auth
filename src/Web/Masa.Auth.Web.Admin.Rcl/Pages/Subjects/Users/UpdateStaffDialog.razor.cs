namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UpdateStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid StaffId { get; set; }

    private StaffDetailDto StaffDetail { get; set; } = new();

    private UpdateStaffDto? _staff;

    private UpdateStaffDto Staff => _staff ?? (_staff ?? StaffDetail);

    private StaffService StaffService => AuthCaller.StaffService;

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
            await GetStaffDetailAsync();
        }
    }

    public async Task GetStaffDetailAsync()
    {
        StaffDetail = await StaffService.GetStaffDetailAsync(StaffId);
    }

    public async Task UpdateStaffAsync()
    {
        Loading = true;
        await StaffService.UpdateStaffAsync(Staff);
        OpenSuccessMessage("Update staff success");
        await OnSubmitSuccess.InvokeAsync();
        await UpdateVisible(false);
        Loading = false;
    }

    public void OpenRemoveStaffDialog()
    {
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await RemoveStaffAsync();
        }, T("Are you sure delete staff data"));
    }

    public async Task RemoveStaffAsync()
    {
        Loading = true;
        await StaffService.RemoveStaffAsync(StaffId);
        OpenSuccessMessage(T("Delete staff data success"));
        Loading = false;
    }
}

