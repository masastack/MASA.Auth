namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceClaim>
{
    public void Configure(EntityTypeBuilder<ApiResourceClaim> builder)
    {
        builder.ToTable(nameof(ApiResourceClaim), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
    }
}
