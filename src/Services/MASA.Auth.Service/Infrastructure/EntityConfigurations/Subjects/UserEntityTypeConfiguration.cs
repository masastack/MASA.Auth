namespace MASA.Auth.Service.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
            builder.HasKey(u => u.Id);
        }
    }
}
