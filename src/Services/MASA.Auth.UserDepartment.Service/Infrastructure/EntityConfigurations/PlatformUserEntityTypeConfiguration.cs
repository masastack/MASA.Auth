using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
{
    public class PlatformUserEntityTypeConfiguration : IEntityTypeConfiguration<PlatformUser>
    {
        public void Configure(EntityTypeBuilder<PlatformUser> builder)
        {
            builder.ToTable("platformUsers", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
