namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations;

public class RoleItemEntityTypeConfiguration : IEntityTypeConfiguration<RoleItem>
{
    public void Configure(EntityTypeBuilder<RoleItem> builder)
    {
        builder.ToTable("role_items", RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);
    }
}

