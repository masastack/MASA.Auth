namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; private set; }

    private User? _user;

    public virtual User User => _user ?? LazyLoader.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to query user data");

    public string JobNumber { get; private set; } = "";

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; set; } = "";

    public StaffTypes StaffType { get; private set; }

    public bool Enabled { get; private set; }

    private ILazyLoader LazyLoader { get; set; } = null!;

    private Staff(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public Staff(Guid userId, string jobNumber, string name, StaffTypes staffType, bool enabled)
    {
        UserId = userId;
        JobNumber = jobNumber;
        Name = name;
        StaffType = staffType;
        Enabled = enabled;
    }

    public void BindUser(User user)
    {

    }
}
