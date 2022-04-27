namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable(nameof(Staff), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.JobNumber).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.Property(s => s.JobNumber).HasMaxLength(20);
        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
        builder.HasOne(s => s.Position).WithOne().HasForeignKey<Staff>(s => s.PositionId);
        builder.HasMany(s => s.DepartmentStaffs).WithOne(a => a.Staff).HasForeignKey(ds => ds.StaffId);
        builder.HasMany(s => s.TeamStaffs).WithOne().HasForeignKey(ts => ts.StaffId);
    }
}

