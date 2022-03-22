namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private int Step { get; set; } = 1;

    private StringNumber Gender { get; set; } = "Male";

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
            Step = 1;
        }   
    }

    public async Task AddOrEditUserAsync()
    {
        Loading = true;
        await UserService.AddUserAsync(User);
        OpenSuccessMessage(T("Add staff data success"));
        await OnSubmitSuccess.InvokeAsync();
        await UpdateVisible(false);
        Loading = false;
    }
}

