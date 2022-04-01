namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class Team
{
    string _search = string.Empty;
    bool _showAdd, _showEdit;
    TeamDetailDto _editTeamDto = new();
    List<TeamDto> _teams = new();
    TeamService TeamService => AuthCaller.TeamService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadTeams();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadTeams()
    {
        _teams = await TeamService.ListAsync(_search);
    }

    private async Task SearchKeyDown(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == "Enter")
        {
            await LoadTeams();
        }
    }

    private async Task EditTeamHandler(Guid id)
    {
        _editTeamDto = await TeamService.GetAsync(id);
        _showEdit = true;
    }

    private async Task OnCreate(TeamDetailDto dto)
    {
        await TeamService.CreateAsync(dto);
        await LoadTeams();
    }

    private async Task OnUpdateBasicInfo(UpdateTeamBasicInfoDto dto)
    {
        await TeamService.UpdateBasicInfo(dto);
        await LoadTeams();
    }

    private async Task OnUpdateAdminPersonnel(UpdateTeamPersonnelDto dto)
    {
        await TeamService.UpdateAdminPersonnel(dto);
        await LoadTeams();
    }

    private async Task OnUpdateMemberPersonnel(UpdateTeamPersonnelDto dto)
    {
        await TeamService.UpdateMemberPersonnel(dto);
        await LoadTeams();
    }

    private async Task OnDelete(Guid id)
    {
        await TeamService.DeleteAsync(id);
        await LoadTeams();
    }
}

