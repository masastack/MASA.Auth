namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class PlatformUser : AuditAggregateRoot<Guid, Guid>
    {
        public Guid PlatformId { get; private set; }

        public Guid UserId { get; private set; }

        public string Account { get; private set; }

        public string? Password { get; private set; }

        public string? Avatar { get; private set; }

        public UserState UserState { get; private set; }

        protected PlatformUser()
        {
            Account = "";
        }

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
}
