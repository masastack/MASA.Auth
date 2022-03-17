namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Organizations;

public class DepartmentStaffEntityTypeConfiguration : IEntityTypeConfiguration<DepartmentStaff>
{
    public void Configure(EntityTypeBuilder<DepartmentStaff> builder)
    {
        builder.ToTable(nameof(DepartmentStaff), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(d => d.Id);
    }
}

