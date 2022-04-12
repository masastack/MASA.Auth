using Masa.Auth.Contracts.Admin.Infrastructure.Enums;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamBasicInfo
{
    [Parameter]
    public TeamBasicInfoDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamBasicInfoDto> ValueChanged { get; set; }

    List<string> _colors = new List<string> { "purple", "green", "yellow", "red", "blue" };
    List<TeamTypeDto> _teamTypes = new()
    {
        new TeamTypeDto { Id = (int)TeamTypes.Normal, Name = TeamTypes.Normal.ToString() }
    };

    private async Task OnNameChanged(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && Value.Name.FirstOrDefault() != Value.Avatar.Name.FirstOrDefault())
        {
            Value.Avatar.Name = name.FirstOrDefault().ToString();
        }
        Value.Name = name;
        await ValueChanged.InvokeAsync(Value);
    }
}
