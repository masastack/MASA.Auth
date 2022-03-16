using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;
using Masa.Auth.Service.Admin.Infrastructure;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class IdentityResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceClaim>
{
    public void Configure(EntityTypeBuilder<IdentityResourceClaim> builder)
    {
        builder.ToTable(nameof(IdentityResourceClaim), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(200).IsRequired();
    }
}
