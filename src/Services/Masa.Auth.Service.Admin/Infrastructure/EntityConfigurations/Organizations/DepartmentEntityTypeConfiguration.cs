// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Organizations;

public class DepartmentEntityTypeConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(nameof(Department), AuthDbContext.ORGANIZATION_SCHEMA);
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => d.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(d => d.Level).IsUnique().HasFilter("Level = 1");
        builder.Property(d => d.Name).HasMaxLength(20).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(255);
        builder.Property(d => d.Level).HasDefaultValue(1);
    }
}
