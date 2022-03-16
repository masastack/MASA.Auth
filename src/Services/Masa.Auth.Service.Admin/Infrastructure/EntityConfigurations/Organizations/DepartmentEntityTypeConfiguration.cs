namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Organizations;

public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(nameof(Department), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.Name).IsUnique();

        builder.Property(d => d.Name).HasMaxLength(20).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(255);
        builder.Property(d => d.Level).HasDefaultValue(1);
    }
}
