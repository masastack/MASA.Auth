// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiResourceClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourceClaim>
{
    public void Configure(EntityTypeBuilder<ApiResourceClaim> builder)
    {
        builder.ToTable(nameof(ApiResourceClaim), AuthDbContext.SSO_SCHEMA).HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(200).IsRequired();
    }
}
