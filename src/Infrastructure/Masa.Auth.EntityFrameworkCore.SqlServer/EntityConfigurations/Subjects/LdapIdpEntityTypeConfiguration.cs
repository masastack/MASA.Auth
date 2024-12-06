// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Subjects;

public class LdapIdpEntityTypeConfiguration : IEntityTypeConfiguration<LdapIdp>
{
    public void Configure(EntityTypeBuilder<LdapIdp> builder)
    {
        builder.Property(p => p.BaseDn).HasMaxLength(255);
        builder.Property(p => p.UserSearchBaseDn).HasMaxLength(255);
        builder.Property(p => p.GroupSearchBaseDn).HasMaxLength(255);
        builder.Property(p => p.RootUserDn).HasMaxLength(255);
        builder.Property(p => p.RootUserPassword).HasMaxLength(255);
    }
}
