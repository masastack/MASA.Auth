namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UpdateUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    private UserDetailDto UserDetail { get; set; } = new ();

    private UpdateUserDto User { get; set; } = new();

    private UserService UserService => AuthCaller.UserService;

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetUserDetailAsync();
        }
    }

    public async Task GetUserDetailAsync()
    {
        UserDetail = await UserService.GetUserDetailAsync(UserId);
        User = UserDetail;
    }

    public async Task UpdateUserAsync()
    {
        Loading = true;
        await UserService.UpdateUserAsync(User);
        OpenSuccessMessage(T("Update user data success"));
        await OnSubmitSuccess.InvokeAsync();
        await UpdateVisible(false);
        Loading = false;
    }

    public void OpenRemoveUserDialog()
    {
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await RemoveUserAsync();
        }, T("Are you sure delete user data"));
    }

    public async Task RemoveUserAsync()
    {
        Loading = true;
        await UserService.RemoveUserAsync(UserId);
        OpenSuccessMessage(T("Delete user data success"));
        Loading = false;
    }
}

