namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class TeamRoleEntityTypeConfiguration : IEntityTypeConfiguration<TeamRole>
    {
        public void Configure(EntityTypeBuilder<TeamRole> builder)
        {
            builder.ToTable("teamRoles", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
