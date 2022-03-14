namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Permissions;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.AppId, p.Code }).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Url).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.Property(p => p.Type).HasConversion(
            v => v.ToString(),
            v => (PermissionType)Enum.Parse(typeof(PermissionType), v)
        );

        builder.HasMany(p => p.PermissionItems).WithOne(pi => pi.Permission);
        builder.HasMany(p => p.RolePermissions).WithOne(rp => rp.Permission);
        builder.HasMany(p => p.UserPermissions).WithOne(up => up.Permission);
        builder.HasMany(p => p.TeamPermissions).WithOne(tp => tp.Permission);
    }
}

