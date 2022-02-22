namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class StaffRoleEntityTypeConfiguration : IEntityTypeConfiguration<StaffRole>
    {
        public void Configure(EntityTypeBuilder<StaffRole> builder)
        {
            builder.ToTable(nameof(StaffRole), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(s => s.Id);
        }
    }
}
