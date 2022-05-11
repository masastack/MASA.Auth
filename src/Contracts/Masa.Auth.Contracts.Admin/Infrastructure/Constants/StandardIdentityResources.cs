// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class StandardIdentityResources
{
    [Description("Your postal address")]
    public static List<string> Adress = new()
    {
        StandardUserClaims.Address
    };

    [Description("Your email address")]
    public static List<string> Email = new()
    {
        StandardUserClaims.Email,
        StandardUserClaims.EmailVerified,
    };

    [Description("Your phone number")]
    public static List<string> Phone = new()
    {
        StandardUserClaims.PhoneNumber,
        StandardUserClaims.PhoneNumberVerified
    };

    [Description("Your user identifier")]
    public static List<string> OpenId = new()
    {
        StandardUserClaims.Subject,
    };

    [Description("User profile")]
    public static List<string> Profile = new()
    {
        StandardUserClaims.Name,
        StandardUserClaims.FamilyName,
        StandardUserClaims.GivenName,
        StandardUserClaims.MiddleName,
        StandardUserClaims.NickName,
        StandardUserClaims.PreferredUserName,
        StandardUserClaims.Profile,
        StandardUserClaims.Picture,
        StandardUserClaims.WebSite,
        StandardUserClaims.Gender,
        StandardUserClaims.BirthDate,
        StandardUserClaims.ZoneInfo,
        StandardUserClaims.Locale,
        StandardUserClaims.UpdatedAt
    };

    static List<IdentityResourceModel>? _identityResources;

    public static List<IdentityResourceModel> IdentityResources => _identityResources ?? (_identityResources = GetIdentityResources());

    static List<IdentityResourceModel> GetIdentityResources()
    {
        var identityResources = new List<IdentityResourceModel>();
        var properties = typeof(StandardIdentityResources).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (var property in properties)
        {
            var userClaims = (List<string>)(property.GetValue(null) ?? throw new Exception("Error standard identity resources data"));
            var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
            identityResources.Add(new IdentityResourceModel(property.Name.ToLower(),description,userClaims));
        }

        return identityResources;
    }
}
