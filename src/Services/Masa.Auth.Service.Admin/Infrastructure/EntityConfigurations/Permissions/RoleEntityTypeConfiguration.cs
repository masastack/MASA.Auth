// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasMany(r => r.Permissions).WithOne(rp => rp.Role);
        builder.HasMany(r => r.ChildrenRoles).WithOne(ri => ri.ParentRole).HasForeignKey(ri => ri.ParentId);
        builder.HasMany(r => r.ParentRoles).WithOne(ri => ri.Role).HasForeignKey(ri => ri.RoleId).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasMany(r => r.Users).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);
        builder.HasMany(r => r.Teams).WithOne(tr => tr.Role).HasForeignKey(tr => tr.RoleId);
        builder.HasOne(r => r.CreateUser).WithMany().HasForeignKey(r => r.Creator).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasOne(r => r.ModifyUser).WithMany().HasForeignKey(r => r.Modifier).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
    }
}

