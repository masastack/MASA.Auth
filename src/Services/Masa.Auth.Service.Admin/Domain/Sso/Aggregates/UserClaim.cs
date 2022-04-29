namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class UserClaim : AuditAggregateRoot<int, Guid>, ISoftDelete
    {
        public bool IsDeleted { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public UserClaimType UserClaimType { get; private set; }

        public UserClaim(string name, string description, UserClaimType userClaimType)
        {
            Name = name;
            Description = description;
            UserClaimType = userClaimType;
        }

        public static implicit operator UserClaimDetailDto(UserClaim userClaim)
        {
            return new UserClaimDetailDto(userClaim.Id, userClaim.Name, userClaim.Description, userClaim.UserClaimType);
        }

        public void Update(string name, string description, UserClaimType userClaimType)
        {
            Name = name;
            Description = description;
            UserClaimType = userClaimType;
        }
    }
}
