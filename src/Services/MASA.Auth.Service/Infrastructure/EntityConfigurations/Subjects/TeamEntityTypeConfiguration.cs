namespace MASA.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable(nameof(Team), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
    }
}

