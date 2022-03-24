namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;
    private Position? _position;
    private DepartmentStaff? _departmentStaff;

    public virtual User User => _user ?? LazyLoader?.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public virtual Position Position => _position ?? LazyLoader?.Load(this, ref _position) ?? new("");

    public virtual DepartmentStaff DepartmentStaff => _departmentStaff ?? LazyLoader?.Load(this, ref _departmentStaff) ?? throw new UserFriendlyException("Failed to get department data");

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
}
