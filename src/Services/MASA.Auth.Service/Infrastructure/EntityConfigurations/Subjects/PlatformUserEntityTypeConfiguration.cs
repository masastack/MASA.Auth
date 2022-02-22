namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PlatformUserEntityTypeConfiguration : IEntityTypeConfiguration<PlatformUser>
    {
        public void Configure(EntityTypeBuilder<PlatformUser> builder)
        {
            builder.ToTable(nameof(PlatformUser), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
