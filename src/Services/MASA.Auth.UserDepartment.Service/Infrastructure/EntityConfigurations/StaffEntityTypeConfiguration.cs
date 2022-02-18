using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
{
    public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("staffs", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
