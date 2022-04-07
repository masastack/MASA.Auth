namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class TeamSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    private List<TeamSelectDto> Teams { get; set; } = new();

    private TeamService TeamService => AuthCaller.TeamService;

    protected override async Task OnInitializedAsync()
    {
        Teams = await TeamService.TeamSelectAsync();
    }
}

