// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiScopeClaimEntityTypeConfiguration : IEntityTypeConfiguration<ApiScopeClaim>
{
    public void Configure(EntityTypeBuilder<ApiScopeClaim> builder)
    {
        builder.ToTable(nameof(ApiScopeClaim), AuthDbContext.SSO_SCHEMA).HasKey(apiScopeClaim => apiScopeClaim.Id);
    }
}
