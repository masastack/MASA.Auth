namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class TeamAdmin
{
    [Parameter]
    public TeamPersonnelDto Value { get; set; } = null!;

    [Parameter]
    public EventCallback<TeamPersonnelDto> ValueChanged { get; set; }

    bool _staffLoading = false;
    List<StaffSelectDto> _staffs = new List<StaffSelectDto>();
    StaffService StaffService => AuthCaller.StaffService;

    public void RemoveAdmin(Guid staffId)
    {
        var index = Value.Staffs.IndexOf(staffId);
        if (index >= 0)
        {
            Value.Staffs.RemoveAt(index);
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
