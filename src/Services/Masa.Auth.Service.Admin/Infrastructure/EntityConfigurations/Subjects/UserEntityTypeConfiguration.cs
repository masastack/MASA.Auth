// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        //mysql use ForMySQLHasCollation("utf8_bin") link https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-charset.html
        builder.Property(c => c.Account).UseCollation("SQL_Latin1_General_CP1_CS_AS");
        builder.HasIndex(u => u.Account).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.IdCard).IsUnique().HasFilter("[IsDeleted] = 0 and IdCard!=''");
        builder.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("[IsDeleted] = 0 and PhoneNumber!=''");
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("[IsDeleted] = 0 and Email!=''");
        builder.Property(u => u.IdCard).HasMaxLength(18);
        builder.Property(u => u.PhoneNumber).HasMaxLength(11);
        builder.OwnsOne(u => u.Address);
        builder.HasMany(u => u.Roles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
        builder.HasMany(u => u.Permissions).WithOne(up => up.User).HasForeignKey(up => up.UserId);
        builder.HasMany(u => u.ThirdPartyUsers).WithOne(tpu => tpu.User).HasForeignKey(tpu => tpu.UserId);
    }
}

