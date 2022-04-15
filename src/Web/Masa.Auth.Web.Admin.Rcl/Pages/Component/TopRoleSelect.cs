namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class TopRoleSelect : RoleSelect
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid OldRoleId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Readonly = true;
        await GetTopRoleSelectAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RoleId != OldRoleId)
        {
            await GetTopRoleSelectAsync();
        }
    }

    private async Task GetTopRoleSelectAsync()
    {
        OldRoleId = RoleId;
        Roles = await RoleService.GetTopRoleSelectAsync(RoleId);
        Value = Roles.Select(r => r.Id).ToList();
    }
}

