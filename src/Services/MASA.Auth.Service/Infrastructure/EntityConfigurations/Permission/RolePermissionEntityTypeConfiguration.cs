namespaceMASA.Auth.Service.Infrastructure.EntityConfigurations;

public class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Aggregate.RolePermission>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregate.RolePermission> builder)
    {
        builder.ToTable(nameof(RolePermission), RolePermissionDbContext.DEFAULT_SCHEMA);
        builder.HasKey(c => c.Id);

        builder.HasOne(a => a.Permission).WithMany().HasForeignKey(x => x.PermissionId);
    }
}

