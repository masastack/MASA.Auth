// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Projects;

public class AppNavigationTagEntityTypeConfiguration : IEntityTypeConfiguration<AppNavigationTag>
{
    public void Configure(EntityTypeBuilder<AppNavigationTag> builder)
    {
        builder.HasKey(ri => ri.Id);
        builder.Property(p => p.AppIdentity).IsRequired().HasMaxLength(255);
        builder.Property(p => p.Tag).HasMaxLength(255);
    }
}
