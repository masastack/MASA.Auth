// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ClientPostLogoutRedirectUriEntityTypeConfiguration : IEntityTypeConfiguration<ClientPostLogoutRedirectUri>
{
    public void Configure(EntityTypeBuilder<ClientPostLogoutRedirectUri> builder)
    {
        builder.ToTable(nameof(ClientPostLogoutRedirectUri), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);
    }
}
