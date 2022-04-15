namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class RoleSelectForRole : RoleSelect
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid OldRoleId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        OldRoleId = RoleId;
        Roles = await RoleService.GetSelectForRoleAsync(RoleId);
    }

    protected override async Task OnParametersSetAsync()
    {
        if(RoleId != OldRoleId)
        {
            OldRoleId = RoleId;
            Roles = await RoleService.GetSelectForRoleAsync(RoleId);
        }
    }
}

