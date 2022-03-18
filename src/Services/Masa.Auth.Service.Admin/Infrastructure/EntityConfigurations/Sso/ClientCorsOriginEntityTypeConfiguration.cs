namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientCorsOriginEntityTypeConfiguration : IEntityTypeConfiguration<ClientCorsOrigin>
{
    public void Configure(EntityTypeBuilder<ClientCorsOrigin> builder)
    {
        builder.ToTable(nameof(ClientCorsOrigin), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Origin).HasMaxLength(150).IsRequired();
    }
}
