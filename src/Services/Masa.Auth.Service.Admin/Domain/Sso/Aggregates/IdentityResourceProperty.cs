namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class IdentityResourceProperty : Property
    {
        public int IdentityResourceId { get; }

        public IdentityResource IdentityResource { get; } = null!;
    }
}
