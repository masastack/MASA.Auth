namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.AppId, p.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Url).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.Property(p => p.Type).HasConversion(
            v => v.ToString(),
            v => (PermissionTypes)Enum.Parse(typeof(PermissionTypes), v)
        );
        builder.HasMany(p => p.RolePermissions).WithOne(rp => rp.Permission);
        builder.HasMany(p => p.UserPermissions).WithOne(up => up.Permission);
        builder.HasMany(p => p.TeamPermissions).WithOne(tp => tp.Permission);
        builder.HasMany(p => p.ParentPermissions).WithMany(pi => pi.ChildPermissions)
            .UsingEntity<PermissionRelation>(
                    cr => cr
                    .HasOne(pr => pr.ParentPermission)
                    .WithMany(p => p.ChildPermissionRelations)
                    .HasForeignKey(pr => pr.ParentPermissionId),
                    cl => cl
                    .HasOne(pr => pr.ChildPermission)
                    .WithMany(p => p.ParentPermissionRelations)
                    .HasForeignKey(pr => pr.ChildPermissionId),
                    cj =>
                    {
                        cj.HasKey(pr => pr.Id);
                        cj.HasIndex(pr => new { pr.ParentPermissionId, pr.ChildPermissionId }).IsUnique().HasFilter("[IsDeleted] = 0");
                    });
    }
}

