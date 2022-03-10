namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Organizations;

public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(nameof(Department), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.Name);

        builder.Property(d => d.Name).HasMaxLength(20).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(255);

        builder.HasMany(d => d.DepartmentStaffs).WithOne(ds => ds.Department);
    }
}
