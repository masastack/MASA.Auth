﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class IdentityResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityResourceClaim>
{
    public void Configure(EntityTypeBuilder<IdentityResourceClaim> builder)
    {
        builder.HasKey(identityResourceClaim => identityResourceClaim.Id);
    }
}
