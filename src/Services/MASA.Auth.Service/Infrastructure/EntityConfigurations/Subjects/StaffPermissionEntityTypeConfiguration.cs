namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffPermissionEntityTypeConfiguration : IEntityTypeConfiguration<StaffPermission>
    {
        public void Configure(EntityTypeBuilder<StaffPermission> builder)
        {
            builder.ToTable(nameof(StaffPermission), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(s => s.Id);
        }
    }
}
