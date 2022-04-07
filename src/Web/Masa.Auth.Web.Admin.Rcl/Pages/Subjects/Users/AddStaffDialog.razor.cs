namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddStaffDto Staff { get; set; } = new();

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

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            Staff = new();           
        }
    }

    public async Task AddStaffAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await StaffService.AddStaffAsync(Staff);
            OpenSuccessMessage(T("Add staff success"));            
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

