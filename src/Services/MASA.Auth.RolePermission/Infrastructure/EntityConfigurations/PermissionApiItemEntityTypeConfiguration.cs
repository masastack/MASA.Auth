namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations;

public class PermissionApiItemEntityTypeConfiguration : IEntityTypeConfiguration<PermissionApiItem>
{
    public void Configure(EntityTypeBuilder<PermissionApiItem> builder)
    {
        builder.ToTable(nameof(PermissionApiItem), RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);


    }
}

