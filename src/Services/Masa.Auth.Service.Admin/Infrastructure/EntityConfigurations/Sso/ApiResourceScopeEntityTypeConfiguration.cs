namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.ToTable(nameof(ApiResourceScope), AuthDbContext.SSO_SCHEMA).HasKey(apiResourceScope => apiResourceScope.Id);
    }
}
