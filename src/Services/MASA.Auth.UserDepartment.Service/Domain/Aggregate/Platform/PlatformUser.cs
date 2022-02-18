namespace MASA.Auth.UserDepartment.Domain.Aggregate.Platform
{
    public class PlatformUser : AuditAggregateRoot<Guid, Guid>
    {
        public string Account { get; set; }

        public string? Password { get; set; }

        public UserState UserState { get; set; }

        public string? Avatar { get; set; }

        public Guid PlatformId { get; set; }

        public PlatformUser(string account) => Account = account;
    }

    public enum UserState
    {
        Enabled,
        Disabled,
    }
}
