namespace Masa.Auth.Service.Domain.Sso.Aggregates
{
    public class IdentityResourceProperty : Property
    {
        public int IdentityResourceId { get; set; }

        public IdentityResource IdentityResource { get; set; } = null!;
    }
}
