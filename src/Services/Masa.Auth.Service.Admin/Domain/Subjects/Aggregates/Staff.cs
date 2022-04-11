namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;
    private Position _position = new("");
    private List<DepartmentStaff> _departmentStaffs = new();
    private List<TeamStaff> _teamStaffs = new();

    public virtual User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public virtual Position Position => _position;

    public virtual IReadOnlyList<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public virtual IReadOnlyList<TeamStaff> TeamStaffs => _teamStaffs;

    public Guid UserId { get; private set; }

    public string JobNumber { get; private set; }

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; private set; }

    public Guid PositionId { get; private set; }

    public StaffTypes StaffType { get; private set; }

    public bool Enabled { get; private set; }

    private ILazyLoader? LazyLoader { get; set; }

    private Staff(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
        Name = "";
        JobNumber = "";
    }

    public Staff(Guid userId, string jobNumber, string name, Guid positionId, StaffTypes staffType, bool enabled)
    {
        UserId = userId;
        JobNumber = jobNumber;
        Name = name;
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public void Update(string name,Guid positionId, StaffTypes staffType,bool enabled)
    {
        Name = name;
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public void AddDepartmentStaff(Guid departmentId)
    {
        _departmentStaffs.Clear();
        _departmentStaffs.Add(new DepartmentStaff(departmentId, Guid.Empty));
    }

    public void AddTeamStaff(List<Guid> teams)
    {
        _teamStaffs.Clear();
        foreach (var teamId in teams)
        {
            _teamStaffs.Add(new TeamStaff(teamId, default, TeamMemberTypes.Member));
        }
    }
}
