namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable(nameof(Team), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
        builder.HasMany(team => team.Staffs).WithOne(teamStaff => teamStaff.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.HasMany(team => team.Permissions).WithOne(teamPermission => teamPermission.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.HasMany(team => team.Roles).WithOne(teamRole => teamRole.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.OwnsOne(team => team.Avatar);
    }
}

