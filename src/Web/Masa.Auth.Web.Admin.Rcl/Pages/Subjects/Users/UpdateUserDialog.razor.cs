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

    private UserDetailDto User { get; set; } = UserDetailDto.Default;

    private UserService UserService => AuthCaller.UserService;

    private async Task UpdateVisible(bool visible)
    {
        if(VisibleChanged.HasDelegate)
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
        if (Visible is true)
        {
            await GetUserDetailAsync();
        }   
    }

    public async Task GetUserDetailAsync()
    {
        User = await UserService.GetUserDetailAsync(UserId);
    }

    public async Task AddOrEditUserAsync()
    {
        Loading = true;
        await UserService.UpdateUserAsync(User);
        OpenSuccessMessage(T("Edit staff data success"));
        await OnSubmitSuccess.InvokeAsync();
        await UpdateVisible(false);
        Loading = false;
    }
}

