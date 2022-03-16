using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceScope>
{
    public void Configure(EntityTypeBuilder<ApiResourceScope> builder)
    {
        builder.ToTable(nameof(ApiResourceScope), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Scope).HasMaxLength(200).IsRequired();
    }
}
