﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientIdPRestrictionEntityTypeConfiguration : IEntityTypeConfiguration<ClientIdPRestriction>
{
    public void Configure(EntityTypeBuilder<ClientIdPRestriction> builder)
    {
        builder.Property(x => x.Provider).HasMaxLength(200).IsRequired();
    }
}
