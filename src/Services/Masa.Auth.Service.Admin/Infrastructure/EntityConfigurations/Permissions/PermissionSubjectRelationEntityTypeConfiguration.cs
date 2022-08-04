// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class PermissionSubjectRelationEntityTypeConfiguration : IEntityTypeConfiguration<PermissionSubjectRelation>
{
    public void Configure(EntityTypeBuilder<PermissionSubjectRelation> builder)
    {
        builder.HasKey(psr => psr.Id);
    }
}

