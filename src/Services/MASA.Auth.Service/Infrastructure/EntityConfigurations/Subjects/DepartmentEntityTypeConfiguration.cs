namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
