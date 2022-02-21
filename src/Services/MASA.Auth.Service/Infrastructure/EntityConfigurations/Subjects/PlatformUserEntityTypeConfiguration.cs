namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class PlatformUserEntityTypeConfiguration : IEntityTypeConfiguration<PlatformUser>
    {
        public void Configure(EntityTypeBuilder<PlatformUser> builder)
        {
            builder.ToTable("platformUsers", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
