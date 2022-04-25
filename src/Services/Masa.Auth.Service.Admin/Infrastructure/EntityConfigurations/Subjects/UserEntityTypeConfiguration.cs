namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.IdCard).HasFilter("[IsDeleted] = 0");
        builder.HasIndex(u => u.PhoneNumber).HasFilter("[IsDeleted] = 0");
        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.Email);
        builder.Property(u => u.IdCard).HasMaxLength(18);
        builder.Property(u => u.PhoneNumber).HasMaxLength(11);
        builder.OwnsOne(u => u.Address);
        builder.HasMany(u => u.Roles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
        builder.HasMany(u => u.Permissions).WithOne(up => up.User).HasForeignKey(up => up.UserId);
        builder.HasMany(u => u.ThirdPartyUsers).WithOne(tpu => tpu.User).HasForeignKey(tpu => tpu.UserId);
    }
}

