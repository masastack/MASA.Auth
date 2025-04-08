// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Permissions;

public class RoleClientEntityTypeConfiguratio : IEntityTypeConfiguration<RoleClient>
{
    public void Configure(EntityTypeBuilder<RoleClient> builder)
    {
        builder.HasKey(rc => new { rc.RoleId, rc.ClientId });
        builder.Property(rc => rc.RoleId).IsRequired();
        builder.Property(rc => rc.ClientId).IsRequired();
    }
}
