namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleSelectForTeam : RoleSelect
{
    [Parameter]
    public Guid TeamId { get; set; }

    [Parameter]
    public int TeamUserCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Roles = await RoleService.GetSelectForTeamAsync(TeamId);
    }

    protected override List<RoleSelectDto> GetRoleSelect()
    {
        return Roles.Where(r => r.QuantityAvailable >= TeamUserCount).ToList();
    }
}

