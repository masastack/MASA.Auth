namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleOwner
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid OldRoleId { get; set; }
         
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    protected RoleOwnerDto Role { get; set; } = new();

    protected RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        await GetRoleOwnerAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if(RoleId != OldRoleId)
        {
           await GetRoleOwnerAsync();
        }
    }

    private async Task GetRoleOwnerAsync()
    {
        OldRoleId = RoleId;
        Role = await RoleService.GetRoleOwnerAsync(RoleId);
    }
}

