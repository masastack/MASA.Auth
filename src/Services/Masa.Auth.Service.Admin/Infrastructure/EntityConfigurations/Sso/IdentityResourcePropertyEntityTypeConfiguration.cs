namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class IdentityResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceProperty>
{
    public void Configure(EntityTypeBuilder<IdentityResourceProperty> builder)
    {
        builder.ToTable(nameof(IdentityResourceProperty), AuthDbContext.SSO_SCHEMA);
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
