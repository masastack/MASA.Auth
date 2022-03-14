namespace Masa.Auth.Service.Infrastructure.EntityConfigurations.Permissions;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(r => r.Id);

        builder.HasMany(r => r.RolePermissions).WithOne(rp => rp.Role);
        builder.HasMany(r => r.RoleItems).WithOne(ri => ri.Role);
    }
}

