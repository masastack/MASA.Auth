namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleSelectForTeam : RoleSelect
{
    [Parameter]
    public int TeamUserCount { get; set; }

    public int Limit => Roles.Where(r => r.Limit != 0).Min(r => r.AvailableQuantity);

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity <= TeamUserCount;

    public async Task ReloadAsync()
    {
        Roles = await RoleService.GetSelectForTeamAsync();
    }
}

