﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Organizations;

public class PositionEntityTypeConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Name).IsUnique().HasFilter("NOT \"IsDeleted\"");
        builder.Property(d => d.Name).HasMaxLength(20);
    }
}
