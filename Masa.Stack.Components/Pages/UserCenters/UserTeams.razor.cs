namespace Masa.Stack.Components.UserCenters;

public partial class UserTeams : MasaComponentBase
{
    public List<TeamModel> Teams { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Teams = await AuthClient.TeamService.GetUserTeamsAsync();
            StateHasChanged();
        }
    }
}
