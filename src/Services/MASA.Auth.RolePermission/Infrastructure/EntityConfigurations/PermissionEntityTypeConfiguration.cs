namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions", RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);
    }
}

