namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto : StaffDto
{
    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public List<Guid> TeamIds { get; set; } = new();

    public new UserDetailDto User { get; set; } = new();

    public string Creator { get; set; } = "";

    public string Modifier { get; set; } = "";

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public StaffDetailDto()
    {
    }

    [JsonConstructor]
    public StaffDetailDto(Guid id, Guid departmentId, string department, Guid positionId, string position, string jobNumber, bool enabled, StaffTypes staffType, List<Guid> teamIds, UserDetailDto user, string creator, string modifier, DateTime creationTime, DateTime? modificationTime) : base(id, department, position, jobNumber, enabled, staffType, user)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
        TeamIds = teamIds;
        User = user;
        Creator = creator;
        Modifier = modifier;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
    }
}


