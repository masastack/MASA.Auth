namespace MASA.Auth.Service.Infrastructure.EntityConfigurations;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Type).HasConversion(
            v => v.ToString(),
            v => (PermissionType)Enum.Parse(typeof(PermissionType), v)
        );

        builder.HasMany(p => p.PermissionItems).WithOne(pi => pi.Permission);
        builder.HasMany(p => p.RolePermissions).WithOne(rp => rp.Permission);
    }
}

