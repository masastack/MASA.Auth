namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable(nameof(Client), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
    }
}
