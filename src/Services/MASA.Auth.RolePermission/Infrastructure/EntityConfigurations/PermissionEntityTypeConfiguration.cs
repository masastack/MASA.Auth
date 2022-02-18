namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);

        builder.HasMany(a => a.PermissionApiItems).WithOne(p => p.Permission);
    }
}

