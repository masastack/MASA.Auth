// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Subjects;

public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Name).IsUnique().HasFilter("NOT \"IsDeleted\"");
        builder.Property(p => p.Name).HasMaxLength(20).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(255);
        builder.HasMany(team => team.TeamStaffs);
        builder.HasMany(team => team.TeamPermissions).WithOne(teamPermission => teamPermission.Team).HasForeignKey(teamPermission => teamPermission.TeamId);
        builder.HasMany(team => team.TeamRoles).WithOne(tr => tr.Team).HasForeignKey(tr => tr.TeamId);
        builder.OwnsOne(team => team.Avatar);
    }
}

