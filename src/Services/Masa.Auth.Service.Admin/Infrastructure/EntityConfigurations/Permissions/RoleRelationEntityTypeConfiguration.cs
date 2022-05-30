// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class RoleRelationEntityTypeConfiguration : IEntityTypeConfiguration<RoleRelation>
{
    public void Configure(EntityTypeBuilder<RoleRelation> builder)
    {
        builder.HasKey(ri => ri.Id);
    }
}

