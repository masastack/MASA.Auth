// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public static class StandardUserClaims
{
    [Description("subject 的缩写，唯一标识，一般为用户 ID")]
    public const string Subject = "sub";
    [Description("姓名")]
    public const string Name = "name";
    [Description("名字")]
    public const string GivenName = "given_name";
    [Description("姓")]
    public const string FamilyName = "family_name";
    [Description("中间名")]
    public const string MiddleName = "middle_name";
    [Description("昵称")]
    public const string NickName = "nickname";
    [Description("希望被称呼的名字")]
    public const string PreferredUserName = "preferred_username";
    [Description("基础资料")]
    public const string Profile = "profile";
    [Description("头像")]
    public const string Picture = "picture";
    [Description("网站链接")]
    public const string WebSite = "website";
    [Description("电子邮箱")]
    public const string Email = "email";
    [Description("邮箱是否被认证")]
    public const string EmailVerified = "email_verified";
    [Description("性别")]
    public const string Gender = "gender";
    [Description("生日")]
    public const string BirthDate = "birthdate";
    [Description("时区")]
    public const string ZoneInfo = "zoneinfo";
    [Description("区域")]
    public const string Locale = "locale";
    [Description("手机号")]
    public const string PhoneNumber = "phone_number";
    [Description("认证手机号")]
    public const string PhoneNumberVerified = "phone_number_verified";
    [Description("地址")]
    public const string Address = "address";
    [Description("详细地址")]
    public const string Formatted = "formatted";
    [Description("街道地址")]
    public const string StreetAddress = "street_address";
    [Description("城市")]
    public const string Locality = "locality";
    [Description("省")]
    public const string Region = "region";
    [Description("邮编")]
    public const string PostalCode = "postal_code";
    [Description("国家")]
    public const string Country = "country";
    [Description("信息更新时间")]
    public const string UpdatedAt = "updated_at";

    static Dictionary<string, string>? _claims;

    public static Dictionary<string, string> Claims => _claims ?? (_claims = GetClaims());

    static Dictionary<string, string> GetClaims()
    {
        var claims = new Dictionary<string, string>();
        var fileds = typeof(StandardUserClaims).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var filed in fileds)
        {
            var value = filed.GetValue(null)?.ToString() ?? "";
            var description = filed.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
            claims.Add(value, description);
        }

        return claims;
    }
}

