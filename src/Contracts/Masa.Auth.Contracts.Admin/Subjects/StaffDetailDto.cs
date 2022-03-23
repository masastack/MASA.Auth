namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public List<Guid> TeamIds { get; set; }

    public UserDetailDto User { get; set; }

    public StaffDetailDto()
    {
        Position = "";
        JobNumber = "";
        TeamIds = new();
        User = new();
    }

    public StaffDetailDto(Guid id, Guid departmentId, Guid positionId, string position, string jobNumber, bool enabled, StaffTypes staffType, List<Guid> teamIds, UserDetailDto user)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        TeamIds = teamIds;
        User = user;
    }
}


