namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class TeamStaffEntityTypeConfiguration : IEntityTypeConfiguration<TeamStaff>
    {
        public void Configure(EntityTypeBuilder<TeamStaff> builder)
        {
            builder.ToTable("teamStaffs", UserDepartmentDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);
        }
    }
}
