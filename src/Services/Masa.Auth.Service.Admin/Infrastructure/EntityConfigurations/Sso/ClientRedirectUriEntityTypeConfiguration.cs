namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientRedirectUriEntityTypeConfiguration : IEntityTypeConfiguration<ClientRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientRedirectUri> builder)
    {
        builder.ToTable(nameof(ClientRedirectUri), AuthDbContext.SSO_SCHEMA);
        builder.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
    }
}
