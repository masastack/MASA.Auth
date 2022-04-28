namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateStaffDto
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public UpsertPositionDto Position { get; set; }

    public List<Guid> Teams { get; set; }

    public UpdateUserDto User { get; set; }

    public UpdateStaffDto()
    {
        JobNumber = "";
        Position = new();
        Teams = new();
        User = new();
    }

    public UpdateStaffDto(Guid id, string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, UpsertPositionDto position, List<Guid> teamIds, UpdateUserDto user)
    {
        Id = id;
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        Position = position;
        Teams = teamIds;
        User = user;
    }

    public static implicit operator UpdateStaffDto(StaffDetailDto staff)
    {
        return new UpdateStaffDto(staff.Id, staff.JobNumber, staff.StaffType, staff.Enabled, staff.DepartmentId, new(staff.PositionId, ""), staff.TeamIds, staff.User);
    }
}
