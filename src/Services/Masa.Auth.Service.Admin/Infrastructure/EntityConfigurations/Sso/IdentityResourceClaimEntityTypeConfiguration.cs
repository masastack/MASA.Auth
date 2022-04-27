namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class IdentityResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceClaim>
{
    public void Configure(EntityTypeBuilder<IdentityResourceClaim> builder)
    {
        builder.ToTable(nameof(IdentityResourceClaim), AuthDbContext.SSO_SCHEMA).HasKey(identityResourceClaim => identityResourceClaim.Id);
    }
}
