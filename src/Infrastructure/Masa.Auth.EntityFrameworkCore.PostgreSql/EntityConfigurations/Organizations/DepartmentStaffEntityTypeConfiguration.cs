// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Organizations;

public class DepartmentStaffEntityTypeConfiguration : IEntityTypeConfiguration<DepartmentStaff>
{
    public void Configure(EntityTypeBuilder<DepartmentStaff> builder)
    {
        builder.HasKey(d => d.Id);
    }
}

