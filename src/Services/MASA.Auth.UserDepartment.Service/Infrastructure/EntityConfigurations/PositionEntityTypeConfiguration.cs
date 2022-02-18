using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
{
    public class PositionEntityTypeConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
