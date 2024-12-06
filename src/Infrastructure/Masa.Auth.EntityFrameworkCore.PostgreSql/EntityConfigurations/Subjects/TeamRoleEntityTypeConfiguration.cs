// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Subjects;

public class TeamRoleEntityTypeConfiguration : IEntityTypeConfiguration<TeamRole>
{
    public void Configure(EntityTypeBuilder<TeamRole> builder)
    {
        builder.HasKey(t => t.Id);
    }
}

