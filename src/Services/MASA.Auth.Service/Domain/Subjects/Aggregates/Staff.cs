namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; private set; }

    private User? _user;

    public virtual User User => LazyLoader.Load(this, ref _user)!;


    public string JobNumber { get; private set; } = "";

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; set; } = "";

    public StaffStates StaffState { get; private set; }

    public StaffTypes StaffType { get; private set; }

    public bool Enabled { get; private set; }

    private ILazyLoader LazyLoader { get; set; } = null!;

    private Staff(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public Staff(Guid userId, string name, string jobNumber, StaffStates staffState, StaffTypes staffType)
    {
        UserId = userId;
        Name = name;
        JobNumber = jobNumber;
        StaffState = staffState;
        StaffType = staffType;
    }
}
