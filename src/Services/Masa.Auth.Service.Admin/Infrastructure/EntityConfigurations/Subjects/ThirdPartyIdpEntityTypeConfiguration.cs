// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<ThirdPartyIdp> builder)
    {
        builder.ToTable(nameof(ThirdPartyIdp), AuthDbContext.SUBJECT_SCHEMA);
        builder.HasKey(tpIdp => tpIdp.Id);
    }
}

