namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffPermissionEntityTypeConfiguration : IEntityTypeConfiguration<StaffPermission>
    {
        public void Configure(EntityTypeBuilder<StaffPermission> builder)
        {
            builder.ToTable("staffPermission", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
