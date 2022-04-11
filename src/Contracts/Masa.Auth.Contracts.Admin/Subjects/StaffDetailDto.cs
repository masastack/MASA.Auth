namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public List<Guid> TeamIds { get; set; }

    public UserDetailDto User { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public StaffDetailDto()
    {
        JobNumber = "";
        TeamIds = new();
        User = new();
        Creator = "";
        Modifier = "";
    }

    [JsonConstructor]
    public StaffDetailDto(Guid id, Guid departmentId, Guid positionId, string jobNumber, bool enabled, StaffTypes staffType, List<Guid> teamIds, UserDetailDto user, string creator, string modifier, DateTime creationTime, DateTime? modificationTime)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        TeamIds = teamIds;
        User = user;
        Creator = creator;
        Modifier = modifier;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
    }
}


