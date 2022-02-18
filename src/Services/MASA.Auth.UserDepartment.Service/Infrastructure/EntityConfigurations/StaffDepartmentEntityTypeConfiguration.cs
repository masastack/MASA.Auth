using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
{
    public class StaffDepartmentEntityTypeConfiguration : IEntityTypeConfiguration<StaffDepartment>
    {
        public void Configure(EntityTypeBuilder<StaffDepartment> builder)
        {
            builder.ToTable("staffDepartments", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
