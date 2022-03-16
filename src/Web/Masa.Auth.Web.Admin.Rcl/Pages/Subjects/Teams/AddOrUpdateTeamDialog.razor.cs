using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddOrEditTeamDialog
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

    private TeamDetailResponse Team { get; set; } = TeamDetailResponse.Default;

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
            if (IsAdd) Team = TeamDetailResponse.Default;
            else await GetTeamDetailAsync();
        }
    }

    public async Task GetTeamDetailAsync()
    {
        var response = await AuthClient.GetTeamDetailAsync(TeamId);
        if (response.Success)
        {
            Team = response.Data;
        }
        else OpenErrorMessage(T("Failed to query teamDetail data:") + response.Message);
    }

    public async Task AddOrEditTeamAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var response = await AuthClient.AddTeamAsync(Team);
            if (response.Success)
            {
                OpenSuccessMessage(T("Add team data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to add team:") + response.Message);
        }
        else
        {
            var response = await AuthClient.EditTeamAsync(Team);
            if (response.Success)
            {
                OpenSuccessMessage(T("Edit team data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to edit team:") + response.Message);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

