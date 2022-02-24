namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyUserEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyUser>
{
    public void Configure(EntityTypeBuilder<ThirdPartyUser> builder)
    {
        builder.ToTable(nameof(ThirdPartyUser), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(c => c.Id);
    }
}

