namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PlatformEntityTypeConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.ToTable("platforms", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
