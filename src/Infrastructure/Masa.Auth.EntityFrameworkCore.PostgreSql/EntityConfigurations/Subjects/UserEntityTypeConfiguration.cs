// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Subjects;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        //mysql use ForMySQLHasCollation("utf8_bin") link https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core-charset.html
        //Account/Email 唯一索引按大小写敏感建约束(挡住 LDAP 同步竞态产生的完全相同账号的重复用户)。
        //大小写变体(ZhangSan/zhangsan)的唯一性由应用层承担:UserUniqueKeyQueries 的 LOWER() 预检查
        //拦住顺序流程,仅并发窗口可能漏过(概率极低,已接受的取舍);SS 侧由 CI 列排序规则在 DB 层直接保证。
        //PG 的 LOWER() 比较不命中本索引,登录/注册等查找为全表扫描,十万级内为毫秒级,同为已接受的代价。
        builder.HasIndex(u => u.Account).IsUnique().HasFilter("NOT \"IsDeleted\"");
        builder.HasIndex(u => u.Name);
        builder.HasIndex(u => u.IdCard).IsUnique().HasFilter("NOT \"IsDeleted\" and \"IdCard\"!=''");
        builder.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("NOT \"IsDeleted\" and \"PhoneNumber\"!=''");
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("NOT \"IsDeleted\" and \"Email\"!=''");
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

