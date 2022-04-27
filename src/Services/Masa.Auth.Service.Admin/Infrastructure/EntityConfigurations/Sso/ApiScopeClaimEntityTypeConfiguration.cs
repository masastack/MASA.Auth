namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiScopeClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeClaim>
{
    public void Configure(EntityTypeBuilder<ApiScopeClaim> builder)
    {
        builder.ToTable(nameof(ApiScopeClaim), AuthDbContext.SSO_SCHEMA).HasKey(apiScopeClaim => apiScopeClaim.Id);
    }
}
