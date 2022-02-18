namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffRoleEntityTypeConfiguration : IEntityTypeConfiguration<StaffRole>
    {
        public void Configure(EntityTypeBuilder<StaffRole> builder)
        {
            builder.ToTable("staffRoles", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
