// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.ComponentModel;

namespace Masa.Auth.Security.OAuth.Providers;

public static class UserClaims
{
    [Description("subject 的缩写，唯一标识，一般为用户 ID")]
    public static readonly string Subject = "sub";

    [Description("姓名")]
    public static readonly string Name = "name";

    [Description("名字")]
    public static readonly string GivenName = "given_name";

    [Description("姓")]
    public static readonly string FamilyName = "family_name";

    [Description("中间名")]
    public static readonly string MiddleName = "middle_name";

    [Description("昵称")]
    public static readonly string NickName = "nickname";

    [Description("希望被称呼的名字")]
    public static readonly string PreferredUserName = "preferred_username";

    [Description("基础资料")]
    public static readonly string Profile = "profile";

    [Description("头像")]
    public static readonly string Picture = "picture";

    [Description("网站链接")]
    public static readonly string WebSite = "website";

    [Description("电子邮箱")]
    public static readonly string Email = "email";

    [Description("邮箱是否被认证")]
    public static readonly string EmailVerified = "email_verified";

    [Description("性别")]
    public static readonly string Gender = "gender";

    [Description("生日")]
    public static readonly string BirthDate = "birthdate";

    [Description("时区")]
    public static readonly string ZoneInfo = "zoneinfo";

    [Description("区域")]
    public static readonly string Locale = "locale";

    [Description("手机号")]
    public static readonly string PhoneNumber = "phone_number";

    [Description("认证手机号")]
    public static readonly string PhoneNumberVerified = "phone_number_verified";

    [Description("地址")]
    public static readonly string Address = "address";

    [Description("详细地址")]
    public static readonly string Formatted = "formatted";

    [Description("街道地址")]
    public static readonly string StreetAddress = "street_address";

    [Description("城市")]
    public static readonly string Locality = "locality";

    [Description("省")]
    public static readonly string Region = "region";

    [Description("邮编")]
    public static readonly string PostalCode = "postal_code";

    [Description("国家")]
    public static readonly string Country = "country";

    [Description("信息更新时间")]
    public static readonly string UpdatedAt = "updated_at";

    [Description("账号")]
    public static readonly string Account = "account";

    [Description("公司")]
    public static readonly string Company = "company";
}
