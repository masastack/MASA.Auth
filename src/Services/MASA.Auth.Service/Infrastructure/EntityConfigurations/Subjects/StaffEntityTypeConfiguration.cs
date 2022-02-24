namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable(nameof(Staff), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(s => s.Id);
    }
}

