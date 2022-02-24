namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class StaffPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable(nameof(UserPermission), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(s => s.Id);
    }
}

