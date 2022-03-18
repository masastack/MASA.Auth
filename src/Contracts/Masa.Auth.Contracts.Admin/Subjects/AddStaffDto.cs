namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddStaffDto
{
    public string JobNumber { get; set; }

    public StaffTypes MemberType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public List<Guid> TeamIds { get; set; }

    public AddUserDto User { get; set; }

    public AddStaffDto(string jobNumber, StaffTypes MemberType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teamIds, AddUserDto user)
    {
        JobNumber = jobNumber;
        MemberType = MemberType;
        Enabled = enabled;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        TeamIds = teamIds;
        User = user;
    }

    public static implicit operator AddStaffDto(StaffDetailDto staff)
    {
        return new AddStaffDto(staff.JobNumber, staff.MemberType, staff.Enabled, staff.DepartmentId, staff.PositionId, staff.Position, staff.TeamIds, staff.User);
    }
}
