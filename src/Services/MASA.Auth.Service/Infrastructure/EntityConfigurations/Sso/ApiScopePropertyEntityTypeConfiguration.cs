namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiScopePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeProperty>
{
    public void Configure(EntityTypeBuilder<ApiScopeProperty> builder)
    {
        builder.ToTable(nameof(ApiScopeProperty)).HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
