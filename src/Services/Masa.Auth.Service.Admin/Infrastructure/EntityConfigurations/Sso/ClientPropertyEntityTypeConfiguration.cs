namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientPropertyEntityTypeConfiguration : IEntityTypeConfiguration<ClientProperty>
{
    public void Configure(EntityTypeBuilder<ClientProperty> builder)
    {
        builder.ToTable(nameof(ClientProperty), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
    }
}
