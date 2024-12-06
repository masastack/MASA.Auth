// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Sso;

public class CustomLoginThirdPartyIdpEntityTypeConfiguration : IEntityTypeConfiguration<CustomLoginThirdPartyIdp>
{
    public void Configure(EntityTypeBuilder<CustomLoginThirdPartyIdp> builder)
    {
        builder.HasOne(cltp => cltp.ThirdPartyIdp).WithMany().HasForeignKey(cltp => cltp.ThirdPartyIdpId);
    }
}
