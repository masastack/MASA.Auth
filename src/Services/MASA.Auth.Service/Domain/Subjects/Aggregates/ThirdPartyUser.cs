namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class ThirdPartyUser : Entity<Guid>
{
    public Guid ThirdPartyPlatformId { get; private set; }

    public Guid UserId { get; private set; }

    public bool Enabled { get; private set; }

    private User? _user;

    public User User
    {
        get => _user ?? throw new UserFriendlyException("Failed to get user data");
        set => _user = value;
    }

    public ThirdPartyUser(Guid thirdPartyPlatformId, Guid userId, bool enabled)
    {
        ThirdPartyPlatformId = thirdPartyPlatformId;
        UserId = userId;
        Enabled = enabled;
    }
}

