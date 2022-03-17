namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamStaffEntityTypeConfiguration : IEntityTypeConfiguration<TeamStaff>
{
    public void Configure(EntityTypeBuilder<TeamStaff> builder)
    {
        builder.ToTable(nameof(TeamStaff), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
    }
}

