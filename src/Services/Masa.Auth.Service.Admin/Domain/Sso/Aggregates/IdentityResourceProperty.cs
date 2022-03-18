namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates
{
    public class IdentityResourceProperty : Property
    {
        public int IdentityResourceId { get; private set; }

        public IdentityResource IdentityResource { get; private set; } = null!;
    }
}
