namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Organizations;

public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(nameof(Department), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(d => d.Id);
    }
}
