// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class ApiScopeEntityTypeConfiguration : IEntityTypeConfiguration<ApiScope>
{
    public void Configure(EntityTypeBuilder<ApiScope> builder)
    {
        builder.ToTable(nameof(ApiScope), AuthDbContext.SSO_SCHEMA).HasKey(apiScope => apiScope.Id);
        builder.HasIndex(apiScope => apiScope.Name).IsUnique();

        builder.Property(apiScope => apiScope.Name).HasMaxLength(200).IsRequired();
        builder.Property(apiScope => apiScope.DisplayName).HasMaxLength(200);
        builder.Property(apiScope => apiScope.Description).HasMaxLength(1000);

        builder.HasMany(apiScope => apiScope.UserClaims).WithOne(apiScope => apiScope.ApiScope).HasForeignKey(apiScope => apiScope.ApiScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
