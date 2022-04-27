namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(r => r.Id);
        builder.HasMany(r => r.Permissions).WithOne(rp => rp.Role);
        builder.HasMany(r => r.ChildrenRoles).WithOne(ri => ri.ParentRole).HasForeignKey(ri => ri.ParentId);
        builder.HasMany(r => r.ParentRoles).WithOne(ri => ri.Role).HasForeignKey(ri => ri.RoleId).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasMany(r => r.Users).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);
        builder.HasMany(r => r.Teams).WithOne(tr => tr.Role).HasForeignKey(tr => tr.RoleId);
        builder.HasOne(r => r.CreateUser).WithMany().HasForeignKey(r => r.Creator).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasOne(r => r.ModifyUser).WithMany().HasForeignKey(r => r.Modifier).OnDelete(DeleteBehavior.ClientSetNull);
    }
}

