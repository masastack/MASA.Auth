namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddSheet
{
    TeamDetailDto _teamDetailDto = new TeamDetailDto();
    int _step = 1;

    protected override void OnInitialized()
    {
        _teamDetailDto.TeamBaseInfo.PropertyChanged += (sender, args) => StateHasChanged();
    }

    public void Dispose()
    {
        _teamDetailDto.TeamBaseInfo.PropertyChanged -= (sender, args) => StateHasChanged();
    }
}
