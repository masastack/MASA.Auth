namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamMember
{
    [EditorRequired]
    [Parameter]
    public TeamPersonnelDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamPersonnelDto> ValueChanged { get; set; }

    bool _staffLoading, _roleLoading;
    List<StaffSelectDto> _staffs = new List<StaffSelectDto>();
    List<StaffSelectDto> _roles = new List<StaffSelectDto>();
    StaffService StaffService => AuthCaller.StaffService;

    private void RemoveMember(Guid staffId)
    {
        var index = Value.Staffs.IndexOf(staffId);
        if (index >= 0)
        {
            Value.Staffs.RemoveAt(index);
        }
    }

    private void RemoveMemberRole(Guid roleId)
    {
        var index = Value.Roles.IndexOf(roleId);
        if (index >= 0)
        {
            Value.Roles.RemoveAt(index);
        }
    }

    private async Task QuerySelectionStaff(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return;
        }

        _staffLoading = true;
        _staffs = await StaffService.GetSelectAsync(search);
        _staffLoading = false;
    }
}
