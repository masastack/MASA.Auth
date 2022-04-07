namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddStaffDto
{
    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; } = true;

    public Guid DepartmentId { get; set; }

    public UpdatePositionDto Position { get; set; }

    public List<Guid> Teams { get; set; }

    public Guid UserId { get; set; }

    public AddUserDto User { get; set; }

    public AddStaffDto()
    {
        JobNumber = "";
        Position = new();
        Teams = new();
        User = new();
    }

    public AddStaffDto(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, UpdatePositionDto position, List<Guid> teamIds, AddUserDto user)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        Position = position;
        Teams = teamIds;
        User = user;
    }
}
