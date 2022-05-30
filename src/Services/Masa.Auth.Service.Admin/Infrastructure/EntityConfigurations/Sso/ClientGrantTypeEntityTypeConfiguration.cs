// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientGrantTypeEntityTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
{
    public void Configure(EntityTypeBuilder<ClientGrantType> builder)
    {
        builder.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
    }
}
