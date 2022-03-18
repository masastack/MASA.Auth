namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto
{
    public Guid StaffId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public MemberTypes MemberType { get; set; }

    public List<Guid> TeamIds { get; set; }

    public UserDetailDto User { get; set; }

    public static StaffDetailDto Default => new StaffDetailDto(Guid.Empty, Guid.Empty, Guid.Empty, "", "", true, default, new(), UserDetailDto.Default);

    public StaffDetailDto(Guid staffId, Guid departmentId, Guid positionId, string position, string jobNumber, bool enabled, MemberTypes MemberType, List<Guid> teamIds, UserDetailDto user)
    {
        StaffId = staffId;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        MemberType = MemberType;
        TeamIds = teamIds;
        User = user;
    }
}


