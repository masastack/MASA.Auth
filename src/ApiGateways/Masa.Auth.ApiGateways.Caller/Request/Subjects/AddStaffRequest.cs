namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class AddStaffRequest
{
    public string JobNumber { get; set; }

    public MemberTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public List<Guid> TeamIds { get; set; }

    public AddUserRequest User { get; set; }

    public AddStaffRequest(string jobNumber, MemberTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teamIds, AddUserRequest user)
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

    public static implicit operator AddStaffRequest(StaffDetailResponse staff)
    {
        return new AddStaffRequest(staff.JobNumber, staff.StaffType, staff.Enabled, staff.DepartmentId, staff.PositionId, staff.Position, staff.TeamIds, staff.User);
    }
}
