﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Organizations;

public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => new { d.Name, d.ParentId });
        builder.HasIndex(d => d.Level);
        builder.Property(d => d.Name).HasMaxLength(20).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(255);
        builder.Property(d => d.Level).HasDefaultValue(1);
    }
}
