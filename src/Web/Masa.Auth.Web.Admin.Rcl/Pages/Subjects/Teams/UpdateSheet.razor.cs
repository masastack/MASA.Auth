namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class UpdateSheet
{
    TeamDetailDto _teamDetailDto = new TeamDetailDto();
    StringNumber _tab = 1;

    protected override void OnInitialized()
    {
        _teamDetailDto.TeamBaseInfo.PropertyChanged += (sender, args) => StateHasChanged();
    }

    public void Dispose()
    {
        _teamDetailDto.TeamBaseInfo.PropertyChanged -= (sender, args) => StateHasChanged();
    }

    private async Task Remove()
    {

    }
}
