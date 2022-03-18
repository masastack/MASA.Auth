namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientIdPRestrictionEntityTypeConfiguration : IEntityTypeConfiguration<ClientIdPRestriction>
{
    public void Configure(EntityTypeBuilder<ClientIdPRestriction> builder)
    {
        builder.ToTable(nameof(ClientIdPRestriction), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
        builder.Property(x => x.Provider).HasMaxLength(200).IsRequired();
    }
}
