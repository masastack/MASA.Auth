namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role), RolePermissionDbContext.DEFAULT_SCHEMA);
            builder.HasKey(c => c.Id);

            builder.HasMany(a => a.RolePermissions).WithOne(p => p.Role);
            builder.HasMany(a => a.RoleItems).WithOne(p => p.Role);
        }
    }
}
