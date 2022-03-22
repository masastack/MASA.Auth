namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddOrUpdateUserDialog
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

    private int Step { get; set; } = 1;

    private StringNumber Gender { get; set; } = "Male";

    private UserDetailDto User { get; set; } = UserDetailDto.Default;

    private UserService UserService => AuthCaller.UserService;

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
            if (IsAdd) User = UserDetailDto.Default;
            else await GetUserDetailAsync();
        }
    }

    public async Task GetUserDetailAsync()
    {
        User = await UserService.GetUserDetailAsync(UserId);
    }

    public async Task AddOrEditUserAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            await UserService.AddUserAsync(User);
            OpenSuccessMessage(T("Add staff data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        else
        {
            await UserService.UpdateUserAsync(User);
            OpenSuccessMessage(T("Edit staff data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        Loading = false;
    }
}

