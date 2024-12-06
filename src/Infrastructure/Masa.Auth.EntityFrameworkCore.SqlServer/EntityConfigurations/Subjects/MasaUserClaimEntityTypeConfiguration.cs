// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Subjects;

public class MasaUserClaimEntityTypeConfiguration : IEntityTypeConfiguration<UserClaimValue>
{
    public void Configure(EntityTypeBuilder<UserClaimValue> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
    }
}
