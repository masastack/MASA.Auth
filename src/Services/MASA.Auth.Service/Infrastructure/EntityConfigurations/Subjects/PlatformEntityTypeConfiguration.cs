namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PlatformEntityTypeConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.ToTable(nameof(Platform), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(p => p.Id);
        }
    }
}
