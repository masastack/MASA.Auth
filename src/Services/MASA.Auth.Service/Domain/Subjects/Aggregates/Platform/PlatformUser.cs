namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class PlatformUser : Entity<Guid>
{
    public Guid PlatformId { get; private set; }

    public Guid UserId { get; private set; }

    public string Account { get; private set; }

    public string? Password { get; private set; }

    public string? Avatar { get; private set; }

    public UserState UserState { get; private set; }

    public PlatformUser(Guid platformId, Guid userId, string account, string? password, string? avatar, UserState userState)
    {
        PlatformId = platformId;
        UserId = userId;
        Account = account;
        Password = password;
        Avatar = avatar;
        UserState = userState;
    }
}

public enum UserState
{
    Enabled,
    Disabled,
}

