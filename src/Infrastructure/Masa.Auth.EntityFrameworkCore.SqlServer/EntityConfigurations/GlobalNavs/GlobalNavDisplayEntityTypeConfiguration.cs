// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.GlobalNavs;

public class GlobalNavDisplayEntityTypeConfiguration : IEntityTypeConfiguration<GlobalNavVisible>
{
    public void Configure(EntityTypeBuilder<GlobalNavVisible> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => x.AppId);
    }
}
