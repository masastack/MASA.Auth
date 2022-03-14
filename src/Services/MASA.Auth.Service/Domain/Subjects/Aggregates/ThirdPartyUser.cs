namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class ThirdPartyUser : Entity<Guid>
{
    public Guid ThirdPartyIdpId { get; private set; }

    public Guid UserId { get; private set; }

    public bool Enabled { get; private set; }

    private User? _user;

    public User User
    {
        get => _user ?? throw new UserFriendlyException("Failed to get user data");
        set => _user = value;
    }

    public ThirdPartyUser(Guid thirdPartyLdp, Guid userId, bool enabled)
    {
        ThirdPartyIdpId = thirdPartyLdp;
        UserId = userId;
        Enabled = enabled;
    }
}

