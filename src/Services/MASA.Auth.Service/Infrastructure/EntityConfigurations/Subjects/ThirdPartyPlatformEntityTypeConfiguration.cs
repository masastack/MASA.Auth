namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyPlatformEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyPlatform>
{
    public void Configure(EntityTypeBuilder<ThirdPartyPlatform> builder)
    {
        builder.ToTable(nameof(ThirdPartyPlatform), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(p => p.Id);
    }
}

