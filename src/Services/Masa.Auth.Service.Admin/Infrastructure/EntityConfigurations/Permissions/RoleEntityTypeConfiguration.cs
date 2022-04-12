namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(r => r.Id);
        builder.HasMany(r => r.Permissions).WithOne(rp => rp.Role);
        builder.HasMany(r => r.ChildrenRoles).WithOne(ri => ri.Role);
        builder.HasMany(r => r.Users).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);
    }
}

