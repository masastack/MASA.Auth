﻿namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(nameof(RolePermission), AuthDbContext.PERMISSION_SCHEMA);
        builder.HasKey(rp => rp.Id);
    }
}

