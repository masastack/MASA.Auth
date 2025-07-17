// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        //mysql use ForMySQLHasCollation("utf8_bin") link https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-charset.html
        //builder.Property(c => c.Account).UseCollation("SQL_Latin1_General_CP1_CS_AS");
        builder.HasIndex(u => u.Account);
        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.IdCard);
        builder.HasIndex(u => u.PhoneNumber);
        builder.HasIndex(u => u.Email);
        builder.Property(u => u.IdCard).HasMaxLength(18);
        builder.Property(u => u.PhoneNumber).HasMaxLength(11);
        builder.OwnsOne(u => u.Address);
        builder.HasMany(u => u.Roles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
        builder.HasMany(u => u.Permissions).WithOne(up => up.User).HasForeignKey(up => up.UserId);
        builder.HasMany(u => u.ThirdPartyUsers).WithOne(tpu => tpu.User).HasForeignKey(tpu => tpu.UserId);
        builder.HasMany(u => u.UserClaims).WithOne(uc => uc.User).HasForeignKey(uc => uc.UserId);
        builder.HasMany(u => u.SystemBusinessDatas).WithOne(uc => uc.User).HasForeignKey(s => s.UserId);
        builder.HasIndex(u => new { u.CreationTime, u.ModificationTime });//.IsDescending(); supported 7.0
        builder.Property(u => u.PasswordType).HasConversion(v => v.Id, v => Enumeration.FromValue<PasswordType>(v)).HasDefaultValue(PasswordType.MD5);
        builder.Property(u => u.ClientId).HasMaxLength(255).HasDefaultValue("");
        builder.HasIndex(u => u.ClientId);
    }
}

