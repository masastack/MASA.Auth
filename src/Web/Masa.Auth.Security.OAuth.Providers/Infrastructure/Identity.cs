// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class Identity
{
    /// <summary>
    /// 唯一标识，一般为用户 ID
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// nickname
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 中间名
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// 姓
    /// </summary>
    public string? FamilyName { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// 希望被称呼的名字
    /// </summary>
    public string? PreferredUserName { get; set; }
    /// <summary>
    /// 基础资料
    /// </summary>
    public string? Profile { get; set; }
    /// <summary>
    /// 头像
    /// </summary>
    public string? Picture { get; set; }
    /// <summary>
    /// 网站链接
    /// </summary>
    public string? WebSite { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public string? BirthDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
    /// <summary>
    /// 详细地址
    /// </summary>
    public string? Formatted { get; set; }
    /// <summary>
    /// 街道地址
    /// </summary>
    public string? StreetAddress { get; set; }
    /// <summary>
    /// 城市
    /// </summary>
    public string? Locality { get; set; }
    /// <summary>
    /// 省
    /// </summary>
    public string? Region { get; set; }
    /// <summary>
    /// 邮编
    /// </summary>
    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Account { get; set; }

    public string? Company { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Identity(string subject)
    {
        Subject = subject;
    }

    public static Identity CreaterDefault(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      principal.FindFirstValue(UserClaims.Subject) ??
                      throw new UserFriendlyException("Unknown user");

        var identity = new Identity(subject);
        identity.Name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue(UserClaims.Email);
        identity.Email = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue(UserClaims.Email);
        identity.Gender = principal.FindFirstValue(ClaimTypes.Gender) ?? principal.FindFirstValue(UserClaims.Gender);
        identity.BirthDate = principal.FindFirstValue(ClaimTypes.DateOfBirth) ?? principal.FindFirstValue(UserClaims.BirthDate);
        identity.PhoneNumber = principal.FindFirstValue(ClaimTypes.MobilePhone) ?? principal.FindFirstValue(UserClaims.PhoneNumber);
        identity.NickName = principal.FindFirstValue(UserClaims.NickName);
        identity.MiddleName = principal.FindFirstValue(UserClaims.MiddleName);
        identity.FamilyName = principal.FindFirstValue(ClaimTypes.Surname) ?? principal.FindFirstValue(UserClaims.FamilyName);
        identity.GivenName = principal.FindFirstValue(ClaimTypes.GivenName) ?? principal.FindFirstValue(UserClaims.GivenName);
        identity.PreferredUserName = principal.FindFirstValue(UserClaims.PreferredUserName);
        identity.Profile = principal.FindFirstValue(UserClaims.Profile);
        identity.Picture = principal.FindFirstValue(UserClaims.Picture);
        identity.WebSite = principal.FindFirstValue(ClaimTypes.Uri) ?? principal.FindFirstValue(UserClaims.WebSite);
        identity.Address = principal.FindFirstValue(UserClaims.Address) ?? principal.FindFirstValue(UserClaims.StreetAddress);
        identity.Formatted = principal.FindFirstValue(UserClaims.Formatted) ?? principal.FindFirstValue(ClaimTypes.StreetAddress);
        identity.StreetAddress = principal.FindFirstValue(ClaimTypes.StreetAddress) ?? principal.FindFirstValue(UserClaims.StreetAddress);
        identity.Locality = principal.FindFirstValue(ClaimTypes.Locality) ?? principal.FindFirstValue(UserClaims.Locality);
        identity.Region = principal.FindFirstValue(ClaimTypes.StateOrProvince) ?? principal.FindFirstValue(UserClaims.Region);
        identity.PostalCode = principal.FindFirstValue(ClaimTypes.PostalCode) ?? principal.FindFirstValue(UserClaims.PostalCode);
        identity.Country = principal.FindFirstValue(ClaimTypes.Country) ?? principal.FindFirstValue(UserClaims.Country);
        identity.Account = principal.FindFirstValue(UserClaims.Account);
        identity.Company = principal.FindFirstValue(UserClaims.Company);
        DateTime.TryParse(principal.FindFirstValue(UserClaims.UpdatedAt), out var dateTime);
        identity.UpdatedAt = dateTime;

        return identity;
    }
}
