namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddOrUpdateStaffDialog
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

    private StaffDetailDto Staff { get; set; } = new();

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
            if (IsAdd) Staff = new();
            else await GetStaffDetailAsync();
        }
    }

    public async Task GetStaffDetailAsync()
    {
        Staff = await StaffService.GetStaffDetailAsync(StaffId);
    }

    public async Task AddOrEditStaffAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            await StaffService.AddStaffAsync(Staff);
            OpenSuccessMessage(T("Add staff success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        else
        {
            await StaffService.UpdateStaffAsync(Staff);
            OpenSuccessMessage("Edit staff success");
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
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
        await StaffService.DeleteStaffAsync(StaffId);
        OpenSuccessMessage(T("Delete staff data success"));
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

