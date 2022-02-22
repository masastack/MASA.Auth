namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class TeamStaffEntityTypeConfiguration : IEntityTypeConfiguration<TeamStaff>
    {
        public void Configure(EntityTypeBuilder<TeamStaff> builder)
        {
            builder.ToTable(nameof(TeamStaff), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(t => t.Id);
        }
    }
}
