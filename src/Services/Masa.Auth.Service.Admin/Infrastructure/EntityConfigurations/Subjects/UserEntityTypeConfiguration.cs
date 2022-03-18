namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.IdCard);
        builder.HasIndex(u => new { u.IdCard, u.IsDeleted }).IsUnique().HasFilter(null);
        builder.HasIndex(u => u.PhoneNumber);
        builder.HasIndex(u => new { u.PhoneNumber, u.IsDeleted }).IsUnique().HasFilter(null);
        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.Email);
        builder.Property(u => u.IdCard).HasMaxLength(18);
        builder.Property(u => u.PhoneNumber).HasMaxLength(11);
        builder.OwnsOne(u => u.Household);
        builder.OwnsOne(u => u.Residential);
    }
}

