using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceProperty>
{
    public void Configure(EntityTypeBuilder<ApiResourceProperty> builder)
    {
        builder.ToTable(nameof(ApiResourceProperty), AuthDbContext.SSO_SCHEMA);
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
