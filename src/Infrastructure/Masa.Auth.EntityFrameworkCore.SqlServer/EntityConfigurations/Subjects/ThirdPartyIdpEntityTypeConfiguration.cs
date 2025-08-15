// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Subjects;

public class ThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<ThirdPartyIdp> builder)
    {
        builder.Property(p => p.ClientId).HasMaxLength(255);
        builder.Property(p => p.ClientSecret).HasMaxLength(2000);
    }
}

