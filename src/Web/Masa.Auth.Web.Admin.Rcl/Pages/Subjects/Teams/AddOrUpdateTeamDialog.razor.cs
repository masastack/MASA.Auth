namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddOrUpdateTeamDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid TeamId { get; set; }

    private bool IsAdd => TeamId == Guid.Empty;

    private TeamDetailDto Team { get; set; } = TeamDetailDto.Default;

    private TeamService TeamService => AuthCaller.TeamService;

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible is true)
        {
            if (IsAdd) Team = TeamDetailDto.Default;
            else await GetTeamDetailAsync();
        }
    }

    public async Task GetTeamDetailAsync()
    {
        Team = await TeamService.GetTeamDetailAsync(TeamId);
    }

    public async Task AddOrEditTeamAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            await TeamService.AddTeamAsync(Team);
            OpenSuccessMessage(T("Add team data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        else
        {
            await TeamService.UpdateTeamAsync(Team);
            OpenSuccessMessage(T("Edit team data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

