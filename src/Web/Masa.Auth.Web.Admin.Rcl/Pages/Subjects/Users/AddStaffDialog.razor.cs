namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private MForm? Form { get; set; }

    private int Step { get; set; } = 1;

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
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            Staff = new();
            Step = 1;
        }
    }

    public async Task AddStaffAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await StaffService.AddAsync(Staff);
            OpenSuccessMessage(T("Add staff success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

