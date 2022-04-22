namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<ThirdPartyIdp> builder)
    {
        builder.Property(p => p.ClientId).HasMaxLength(255);
        builder.Property(p => p.ClientSecret).HasMaxLength(255);
    }
}

