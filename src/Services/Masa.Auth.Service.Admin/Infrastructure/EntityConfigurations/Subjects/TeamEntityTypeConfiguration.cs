namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable(nameof(Team), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.HasMany(team => team.Staffs).WithOne(teamStaff => teamStaff.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.HasMany(team => team.Permissions).WithOne(teamPermission => teamPermission.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.HasMany(team => team.Roles).WithOne(teamRole => teamRole.Team).HasForeignKey(teamStaff => teamStaff.TeamId);
        builder.OwnsOne(team => team.Avatar);
    }
}

