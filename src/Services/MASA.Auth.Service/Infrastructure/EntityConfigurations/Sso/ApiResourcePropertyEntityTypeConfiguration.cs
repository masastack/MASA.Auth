namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ApiResourcePropertyEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceProperty>
{
    public void Configure(EntityTypeBuilder<ApiResourceProperty> builder)
    {
        builder.ToTable(nameof(ApiResourceProperty));
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
