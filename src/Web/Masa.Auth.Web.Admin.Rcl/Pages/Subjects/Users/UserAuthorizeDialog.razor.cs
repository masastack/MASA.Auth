namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UserAuthorizeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    public UpdateUserAuthorizationDto Authorization { get; set; } = new();

    public UserDetailDto User { get; set; } = new();

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
        if (Visible)
        {
            User = await UserService.GetDetailAsync(UserId);
            Authorization = new(User.Id, User.RoleIds, User.Permissions);
        }
    }

    private void PermissionsChanged(Dictionary<Guid,bool> permissiionMap)
    {
        Authorization.Permissions = permissiionMap.Select(kv => new UserPermissionDto(kv.Key,kv.Value))
                                                   .ToList();
    }

    private async Task UpdateAuthorizationAsync()
    {
        Loading = true;
        await UserService.UpdateAuthorizationAsync(Authorization);
        OpenSuccessMessage(T("Successfully set user permissions"));
        Loading = false;
    }
}

