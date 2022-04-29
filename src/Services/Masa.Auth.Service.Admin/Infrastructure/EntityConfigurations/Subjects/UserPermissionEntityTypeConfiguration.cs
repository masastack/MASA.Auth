// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class UserPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable(nameof(UserPermission), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(s => s.Id);
    }
}

