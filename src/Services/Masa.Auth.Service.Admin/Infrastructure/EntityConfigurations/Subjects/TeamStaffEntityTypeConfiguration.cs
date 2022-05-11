// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamStaffEntityTypeConfiguration : IEntityTypeConfiguration<TeamStaff>
{
    public void Configure(EntityTypeBuilder<TeamStaff> builder)
    {
        builder.ToTable(nameof(TeamStaff), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(t => t.Id);
    }
}

