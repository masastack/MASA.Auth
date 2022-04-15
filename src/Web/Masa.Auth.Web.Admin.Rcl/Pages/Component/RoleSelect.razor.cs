namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public string Lable { get; set; } = "";

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    protected List<RoleSelectDto> Roles { get; set; } = new();

    protected RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        Roles = await RoleService.GetSelectForUserAsync();
    }

    protected virtual List<RoleSelectDto> GetRoleSelect() => Roles;

    protected void RemoveRole(RoleSelectDto role)
    {
        if(Readonly is false)
        {
            Value.Remove(role.Id);
        }
    }
}

