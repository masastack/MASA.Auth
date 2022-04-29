// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class TeamPermissionEntityTypeConfiguration : IEntityTypeConfiguration<TeamPermission>
{
    public void Configure(EntityTypeBuilder<TeamPermission> builder)
    {
        builder.ToTable(nameof(TeamPermission), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(u => u.Id);
    }
}

