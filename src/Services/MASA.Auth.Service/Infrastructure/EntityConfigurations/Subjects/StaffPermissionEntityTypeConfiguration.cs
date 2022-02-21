namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffPermissionEntityTypeConfiguration : IEntityTypeConfiguration<StaffPermission>
    {
        public void Configure(EntityTypeBuilder<StaffPermission> builder)
        {
            builder.ToTable("staffPermission", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
