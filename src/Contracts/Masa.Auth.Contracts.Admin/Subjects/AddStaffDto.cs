namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddStaffDto
{
    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public List<Guid> Teams { get; set; }

    public AddUserDto User { get; set; }

    public AddStaffDto()
    {
        JobNumber = "";
        Position = "";
        Teams = new();
        User = new();
    }

    public AddStaffDto(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teamIds, AddUserDto user)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        Teams = teamIds;
        User = user;
    }
}
