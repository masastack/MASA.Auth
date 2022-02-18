namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffDepartmentEntityTypeConfiguration : IEntityTypeConfiguration<DepartmentStaff>
    {
        public void Configure(EntityTypeBuilder<DepartmentStaff> builder)
        {
            builder.ToTable("staffDepartments", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
