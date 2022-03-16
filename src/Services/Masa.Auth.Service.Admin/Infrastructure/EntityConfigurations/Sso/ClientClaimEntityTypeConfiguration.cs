namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientClaimEntityTypeConfiguration : IEntityTypeConfiguration<ClientClaim>
{
    public void Configure(EntityTypeBuilder<ClientClaim> builder)
    {
        builder.ToTable(nameof(ClientClaim), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(250).IsRequired();
    }
}
