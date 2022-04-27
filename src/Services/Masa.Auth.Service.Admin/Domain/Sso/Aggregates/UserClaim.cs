namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class UserClaim : Entity<int>
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public UserClaimType UserClaimType { get; private set; }

        public UserClaim(string name, string description, UserClaimType userClaimType)
        {
            Name = name;
            Description = description;
            UserClaimType = userClaimType;
        }
    }
}
