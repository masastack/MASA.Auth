namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>
{
    private User? _user;

    private Position? _position;

    public virtual User User => _user ?? LazyLoader.Load(this, ref _user) ?? throw new UserFriendlyException("Failed to get user data");

    public Position Position
    {
        get => _position ?? new Position("");
        private set => _position = value;
    }

    public Guid UserId { get; private set; }

    public string JobNumber { get; private set; } = "";

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; private set; } = "";

    public Guid PositionId { get; private set; }

    public MemberTypes MemberType { get; private set; }

    public bool Enabled { get; private set; }

    private ILazyLoader LazyLoader { get; set; } = null!;

    private Staff(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public Staff(string jobNumber, string name, MemberTypes MemberType, bool enabled)
    {
        JobNumber = jobNumber;
        Name = name;
        MemberType = MemberType;
        Enabled = enabled;
    }

    public void BindUser(User user)
    {
        if (_user is null)
        {
            _user = user;
            return;
        }
        _user.Update();
    }
}
