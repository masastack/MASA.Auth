// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Permissions;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasMany(r => r.Permissions).WithOne(rp => rp.Role).HasForeignKey(rp => rp.RoleId);
        builder.HasMany(r => r.ChildrenRoles).WithOne(ri => ri.ParentRole).HasForeignKey(ri => ri.ParentId);
        builder.HasMany(r => r.ParentRoles).WithOne(ri => ri.Role).HasForeignKey(ri => ri.RoleId).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasMany(r => r.Users).WithOne(ur => ur.Role).HasForeignKey(ur => ur.RoleId);
        builder.HasMany(r => r.Teams).WithOne(tr => tr.Role).HasForeignKey(tr => tr.RoleId);      
    }
}

