namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamMember
{
    [EditorRequired]
    [Parameter]
    public TeamPersonnelDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamPersonnelDto> ValueChanged { get; set; }

    List<StaffSelectDto> _staffs = new List<StaffSelectDto>();

    public void RemoveMember(Guid staffId)
    {
        var index = Value.Staffs.IndexOf(staffId);
        if (index >= 0)
        {
            Value.Staffs.RemoveAt(index);
        }
    }
}
