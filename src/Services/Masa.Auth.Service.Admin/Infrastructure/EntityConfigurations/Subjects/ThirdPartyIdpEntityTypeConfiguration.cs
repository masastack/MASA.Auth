namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<ThirdPartyIdp> builder)
    {
        builder.ToTable(nameof(ThirdPartyIdp), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(tpIdp => tpIdp.Id);
    }
}

