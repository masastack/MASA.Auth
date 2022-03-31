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
        builder.HasMany(team => team.TeamStaffs);
        builder.HasMany(team => team.TeamPermissions).WithOne(teamPermission => teamPermission.Team);
        builder.HasMany(team => team.TeamRoles);
        builder.OwnsOne(team => team.Avatar);
    }
}

