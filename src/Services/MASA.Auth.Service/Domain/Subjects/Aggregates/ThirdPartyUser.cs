namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class ThirdPartyUser : Entity<Guid>
{
    public Guid ThirdPartyPlatformId { get; private set; }

    public Guid UserId { get; private set; }

    public UserStates UserState { get; private set; }

    private ThirdPartyUser()
    {
    }

    public ThirdPartyUser(Guid thirdPartyPlatformId, Guid userId, UserStates userState)
    {
        ThirdPartyPlatformId = thirdPartyPlatformId;
        UserId = userId;
        UserState = userState;
    }
}

