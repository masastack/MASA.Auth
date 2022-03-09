namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Sso;

public class ClientRedirectUriEntityTypeConfiguration : IEntityTypeConfiguration<ClientRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientRedirectUri> builder)
    {
        builder.ToTable(nameof(ClientRedirectUri));
        builder.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
    }
}
