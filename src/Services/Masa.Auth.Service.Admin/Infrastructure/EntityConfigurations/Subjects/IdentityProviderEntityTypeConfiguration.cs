// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class IdentityProviderEntityTypeConfiguration : IEntityTypeConfiguration<IdentityProvider>
{
    public void Configure(EntityTypeBuilder<IdentityProvider> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Name).HasFilter("[IsDeleted] = 0");
        builder.Property(p => p.Name).HasMaxLength(20);
        builder.Property(p => p.DisplayName).HasMaxLength(20);
    }
}
