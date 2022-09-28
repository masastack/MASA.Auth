// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class Identity
{
    /// <summary>
    /// Unique identifier, usually the user ID
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// actual name
    /// </summary>
    public string? Name { get; set; }

    public string? NickName { get; set; }

    public string? MiddleName { get; set; }

    public string? FamilyName { get; set; }

    public string? GivenName { get; set; }

    public string? PreferredUserName { get; set; }

    /// <summary>
    /// Basic information
    /// </summary>
    public string? Profile { get; set; }

    /// <summary>
    /// avatar
    /// </summary>
    public string? Picture { get; set; }

    public string? WebSite { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public string? BirthDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    /// <summary>
    /// detailed address
    /// </summary>
    public string? Formatted { get; set; }

    public string? StreetAddress { get; set; }

    /// <summary>
    /// city
    /// </summary>
    public string? Locality { get; set; }

    /// <summary>
    /// Province
    /// </summary>
    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Account { get; set; }

    public string? Company { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Issuer { get; set; } = "";

    [JsonIgnore]
    public IDictionary<string,string?> Properties { get; set; }

    public Identity(string subject)
    {
        Subject = subject;
        Properties = new Dictionary<string, string?>();
    }

    public static Identity CreaterDefault(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      principal.FindFirstValue(UserClaims.Subject) ??
                      throw new UserFriendlyException("Unknown user");

        var identity = new Identity(subject);
        identity.Name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue(UserClaims.Name);
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
