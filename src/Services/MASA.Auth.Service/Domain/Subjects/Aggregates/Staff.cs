namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;

    private Position? _position;

    public Guid UserId { get; private set; }

    public virtual User User => _user ?? LazyLoader.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to query user data");

    public string JobNumber { get; private set; } = "";

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; private set; } = "";

    public Guid PositionId { get; private set; }

    public Position Position
    {
        get => _position ?? new Position("");
        set => _position = value;
    }

    public StaffTypes StaffType { get; private set; }

    public bool Enabled { get; private set; }

    private ILazyLoader LazyLoader { get; set; } = null!;

    private Staff(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
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

    public void BindUser(User user)
    {

    }
}
