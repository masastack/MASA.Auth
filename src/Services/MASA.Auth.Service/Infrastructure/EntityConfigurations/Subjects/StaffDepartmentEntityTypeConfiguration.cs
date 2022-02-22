namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffDepartmentEntityTypeConfiguration : IEntityTypeConfiguration<DepartmentStaff>
    {
        public void Configure(EntityTypeBuilder<DepartmentStaff> builder)
        {
            builder.ToTable(nameof(DepartmentStaff), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(d => d.Id);
        }
    }
}
