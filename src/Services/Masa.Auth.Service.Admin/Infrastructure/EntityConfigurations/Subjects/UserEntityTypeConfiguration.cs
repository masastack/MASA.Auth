namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.IdCard).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.Property(u => u.IdCard).HasMaxLength(18);
        builder.Property(u => u.PhoneNumber).HasMaxLength(11);

        builder.OwnsOne(u => u.Address);
    }
}

