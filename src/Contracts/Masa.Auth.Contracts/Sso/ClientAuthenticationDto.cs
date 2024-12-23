// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans;

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientAuthenticationDto
{
    List<CheckItemDto<string>> _grantTypes = new();
    List<string> _allowedGrantTypes = new();

    public Guid Id { get; set; }

    public string RedirectUri { get; set; } = string.Empty;

    public List<string> RedirectUris { get; set; } = new();

    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public List<string> PostLogoutRedirectUris { get; set; } = new();

    public string FrontChannelLogoutUri { get; set; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool EnableLocalLogin { get; set; }

    public string IdentityProviderRestriction { get; set; } = string.Empty;

    public List<string> IdentityProviderRestrictions { get; set; } = new();

    public int? UserSsoLifetime { get; set; }

    public List<string> AllowedGrantTypes
    {
        get
        {
            _allowedGrantTypes = GrantTypes.Where(grant => grant.Selected).Select(grant => grant.Id).ToList();
            return _allowedGrantTypes;
        }
        set
        {
            _grantTypes.ForEach(grant =>
            {
                grant.Selected = value.Contains(grant.Id);
            });
            _allowedGrantTypes = value;
        }
    }

    public List<CheckItemDto<string>> GrantTypes => _grantTypes;

    public ClientAuthenticationDto()
    {
        // 获取当前类的类型
        Type type = typeof(GrantType);

        // 获取所有字段
        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            if (fieldInfo.IsPublic && fieldInfo.IsLiteral && fieldInfo.FieldType == typeof(string))
            {
                var descriptionAttribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
                var value = $"{fieldInfo.GetValue(default)}";
                _grantTypes.Add(new CheckItemDto<string>()
                {
                    Id = value,
                    DisplayValue = descriptionAttribute?.Description ?? value,
                });
            }
        }
    }
}
