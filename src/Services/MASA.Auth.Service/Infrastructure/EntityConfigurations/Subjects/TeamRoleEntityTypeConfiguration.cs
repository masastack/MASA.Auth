namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class TeamRoleEntityTypeConfiguration : IEntityTypeConfiguration<TeamRole>
    {
        public void Configure(EntityTypeBuilder<TeamRole> builder)
        {
            builder.ToTable(nameof(TeamRole), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(t => t.Id);
        }
    }
}
