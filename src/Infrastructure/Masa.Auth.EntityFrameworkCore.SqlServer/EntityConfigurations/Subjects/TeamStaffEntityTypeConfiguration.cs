// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Subjects;

public class TeamStaffEntityTypeConfiguration : IEntityTypeConfiguration<TeamStaff>
{
    public void Configure(EntityTypeBuilder<TeamStaff> builder)
    {
        builder.HasKey(t => t.Id);
    }
}

