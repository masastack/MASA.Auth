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

    private bool Fill { get; set; }

    private MForm? Form { get; set; }

    private StringNumber Gender { get; set; } = "Male";

    private AddUserDto User { get; set; } = new();

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
        if(Form is not null)
        {
            await Form.ResetAsync();
        }   
    }

    protected override void OnParametersSet()
    {
        if (Visible is true)
        {
            Step = 1;
            Fill = false;
            User = new();
        }
    }

    public async Task AddUserAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            User.Avatar = "/_content/Masa.Auth.Web.Admin.Rcl/img/subject/user.svg";
            await UserService.AddAsync(User);
            OpenSuccessMessage(T("Add user data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

