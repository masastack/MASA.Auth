namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ClientGrantTypeEntityTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
{
    public void Configure(EntityTypeBuilder<ClientGrantType> builder)
    {
        builder.ToTable(nameof(ClientGrantType));
        builder.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
    }
}
