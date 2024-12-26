// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Subjects;

public class UserSystemBusinessDataEntityTypeConfiguration : IEntityTypeConfiguration<UserSystemBusinessData>
{
    public void Configure(EntityTypeBuilder<UserSystemBusinessData> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => new { u.UserId, u.SystemId });
    }
}
