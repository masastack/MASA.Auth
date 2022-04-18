namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class RoleSelectForRole : RoleSelect
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid OldRoleId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Lable = T("Inherited Role");
        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RoleId != OldRoleId)
        {
            await ReloadAsync();
        }
    }

    public async Task ReloadAsync()
    {
        OldRoleId = RoleId;
        Roles = await RoleService.GetSelectForRoleAsync(RoleId);
    }
}

