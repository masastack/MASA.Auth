using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
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
