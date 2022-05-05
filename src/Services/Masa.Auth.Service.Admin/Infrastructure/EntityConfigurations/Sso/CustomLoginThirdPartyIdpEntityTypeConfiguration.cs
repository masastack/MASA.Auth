namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class CustomLoginThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<CustomLoginThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<CustomLoginThirdPartyIdp> builder)
    {
        builder.ToTable(nameof(CustomLoginThirdPartyIdp), AuthDbContext.SSO_SCHEMA).HasKey(customLoginThirdPartyIdp => customLoginThirdPartyIdp.Id);
    }
}
