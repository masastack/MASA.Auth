namespace Masa.Auth.Sdk.Request.Subjects;

public class EditStaffRequest
{
    public Guid StaffId { get; set; }

    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public List<Guid> TeamIds { get; set; }

    public EditUserRequest User { get; set; }

    public EditStaffRequest(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teamIds, EditUserRequest user)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        TeamIds = teamIds;
        User = user;
    }

    public static implicit operator EditStaffRequest(StaffDetailResponse staff)
    {
        return new EditStaffRequest(staff.JobNumber, staff.StaffType, staff.Enabled, staff.DepartmentId, staff.PositionId, staff.Position, staff.TeamIds, staff.User);
    }
}
