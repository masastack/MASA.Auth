namespace MASA.Auth.RolePermission.Infrastructure.EntityConfigurations;

public class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Aggregate.RolePermission>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregate.RolePermission> builder)
    {
        builder.ToTable("role_permissions", RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);
    }
}

