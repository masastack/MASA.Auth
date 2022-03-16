namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientScopeEntityTypeConfiguration : IEntityTypeConfiguration<ClientScope>
{
    public void Configure(EntityTypeBuilder<ClientScope> builder)
    {
        builder.ToTable(nameof(ClientScope), AuthDbContext.SSO_SCHEMA);
        builder.Property(x => x.Scope).HasMaxLength(200).IsRequired();
    }
}
