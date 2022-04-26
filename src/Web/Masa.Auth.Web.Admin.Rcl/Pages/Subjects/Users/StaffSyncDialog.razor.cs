namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class StaffSyncDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

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

    private async Task StaffSyncAsync()
    {
        await Task.CompletedTask;
    }
}

