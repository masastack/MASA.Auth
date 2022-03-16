using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;
using Masa.Auth.Service.Admin.Infrastructure;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiScopePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeProperty>
{
    public void Configure(EntityTypeBuilder<ApiScopeProperty> builder)
    {
        builder.ToTable(nameof(ApiScopeProperty), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
