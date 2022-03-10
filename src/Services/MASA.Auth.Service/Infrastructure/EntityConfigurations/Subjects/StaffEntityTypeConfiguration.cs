namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable(nameof(Staff), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.JobNumber).IsUnique();

        builder.Property(s => s.JobNumber).HasMaxLength(20);
        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
    }
}

