// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceSecretEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceSecret>
{
    public void Configure(EntityTypeBuilder<ApiResourceSecret> builder)
    {
        builder.ToTable(nameof(ApiResourceSecret), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);

        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(250).IsRequired();
    }
}
