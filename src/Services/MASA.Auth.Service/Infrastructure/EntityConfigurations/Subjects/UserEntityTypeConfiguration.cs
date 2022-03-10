namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.IDCard).IsUnique();

        builder.Property(u => u.IDCard).HasMaxLength(18);
    }
}

