namespaceMASA.Auth.Service.Infrastructure.EntityConfigurations;

public class PermissionItemEntityTypeConfiguration : IEntityTypeConfiguration<PermissionItem>
{
    public void Configure(EntityTypeBuilder<PermissionItem> builder)
    {
        builder.ToTable(nameof(PermissionItem), RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);


    }
}

