namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class AddRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddRoleDto Role { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    private MForm? Form { get; set; }

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
            Role = new();
        }
    }

    public async Task AddRoleAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await RoleService.AddAsync(Role);
            OpenSuccessMessage(T("Add role data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

