namespace MASA.Auth.Service.Infrastructure.EntityConfigurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(c => c.Id);

        builder.HasMany(a => a.RolePermissions).WithOne(p => p.Role);
        builder.HasMany(a => a.RoleItems).WithOne(p => p.Role);
    }
}

