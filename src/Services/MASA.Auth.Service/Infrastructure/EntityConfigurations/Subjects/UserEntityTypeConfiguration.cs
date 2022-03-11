namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.IdCard).IsUnique();

        builder.Property(u => u.IdCard).HasMaxLength(18);

        builder.OwnsOne(u => u.Household);
        builder.OwnsOne(u => u.Residential);
    }
}

