namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("teams", AuthDbContext.PERMISSION_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
