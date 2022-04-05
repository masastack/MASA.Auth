namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;
    private Position? _position;
    private List<DepartmentStaff>? _departmentStaffs;
    private List<TeamStaff>? _teamStaffs;

    public virtual User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public virtual Position Position => _position ?? LazyLoader?.Load(this, ref _position) ?? new("");

    public virtual List<DepartmentStaff> DepartmentStaffs => _departmentStaffs ?? (_departmentStaffs =new());//LazyLoader?.Load(this, ref _departmentStaffs) ?? throw new UserFriendlyException("Failed to get department data");

    public virtual List<TeamStaff> TeamStaffs => _teamStaffs ?? (_teamStaffs = new());

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

    public void AddDepartmentStaff(Guid departmentId)
    {
        DepartmentStaffs.Add(new DepartmentStaff(departmentId, Guid.Empty));
    }

    public void AddTeamStaff(List<Guid> teams)
    {
        foreach(var teamId in teams)
        {
            TeamStaffs.Add(new TeamStaff(teamId,default,UserId,TeamMemberTypes.Member));
        }
    }
}
