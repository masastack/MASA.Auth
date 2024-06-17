// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.GlobalNavs;

public class GlobalNavDisplayEntityTypeConfiguration : IEntityTypeConfiguration<GlobalNavVisible>
{
    public void Configure(EntityTypeBuilder<GlobalNavVisible> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ClientId).HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => x.AppId).HasFilter("[IsDeleted] = 0");
    }
}
