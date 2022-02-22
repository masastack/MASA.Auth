
namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable(nameof(Department), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(d => d.Id);
        }
    }
}
