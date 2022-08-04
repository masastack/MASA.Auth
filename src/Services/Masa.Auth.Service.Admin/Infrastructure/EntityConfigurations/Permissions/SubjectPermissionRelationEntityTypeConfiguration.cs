// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class SubjectPermissionRelationEntityTypeConfiguration : IEntityTypeConfiguration<SubjectPermissionRelation>
{
    public void Configure(EntityTypeBuilder<SubjectPermissionRelation> builder)
    {
        builder.HasKey(spr => spr.Id);
    }
}

