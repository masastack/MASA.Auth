using Masa.Auth.Contracts.Admin.Infrastructure.Enums;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamBaseinfo
{
    [Parameter]
    public TeamBaseInfoDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamBaseInfoDto> ValueChanged { get; set; }

    List<string> _colors = new List<string> { "purple", "green", "yellow", "red", "blue" };
    List<TeamTypeDto> _teamTypes = new()
    {
        new TeamTypeDto { Id = (int)TeamTypes.Normal, Name = TeamTypes.Normal.ToString() }
    };
}
