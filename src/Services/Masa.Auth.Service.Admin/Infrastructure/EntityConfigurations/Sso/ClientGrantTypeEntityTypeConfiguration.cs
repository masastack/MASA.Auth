using Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientGrantTypeEntityTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
{
    public void Configure(EntityTypeBuilder<ClientGrantType> builder)
    {
        builder.ToTable(nameof(ClientGrantType), AuthDbContext.SSO_SCHEMA);
        builder.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
    }
}
