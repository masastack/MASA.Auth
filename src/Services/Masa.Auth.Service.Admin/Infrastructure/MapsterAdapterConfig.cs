// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Service.Admin.Infrastructure;

public static class MapsterAdapterConfig
{
    public static void TypeAdapter()
    {
        TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);

        TypeAdapterConfig<string, ClientGrantType>.NewConfig().MapWith(src => new ClientGrantType(src));
        TypeAdapterConfig<string, ClientRedirectUri>.NewConfig().MapWith(src => new ClientRedirectUri(src));
        TypeAdapterConfig<string, ClientPostLogoutRedirectUri>.NewConfig().MapWith(src => new ClientPostLogoutRedirectUri(src));
        TypeAdapterConfig<string, ClientScope>.NewConfig().MapWith(src => new ClientScope(src));
        TypeAdapterConfig<ClientGrantType, string>.NewConfig().MapWith(src => src.GrantType);
        TypeAdapterConfig<ClientRedirectUri, string>.NewConfig().MapWith(src => src.RedirectUri);
        TypeAdapterConfig<ClientPostLogoutRedirectUri, string>.NewConfig().MapWith(src => src.PostLogoutRedirectUri);
        TypeAdapterConfig<ClientScope, string>.NewConfig().MapWith(src => src.Scope);
        TypeAdapterConfig<DateOnly?, DateTime?>.NewConfig().MapWith(src => src.HasValue ? src.Value.ToDateTime(TimeOnly.Parse("00:00")) : null);
        TypeAdapterConfig<DateTime?, DateOnly?>.NewConfig().MapWith(src => src.HasValue ? DateOnly.FromDateTime(src.Value) : null);
        TypeAdapterConfig<string, ClientGrantType>.NewConfig().MapWith(item => new ClientGrantType(item));
        //TypeAdapterConfig<ClientDetailDto, Client>.NewConfig().IgnoreIf((src, dest) => dest.AllowedScopes.Any(), dest => dest.AllowedScopes);

        TypeAdapterConfig<LdapDetailDto, LdapOptions>.ForType()
            .Map(dest => dest.ServerPort, src => src.IsLdaps ? 0 : src.ServerPort)
            .Map(dest => dest.ServerPortSsl, src => src.IsLdaps ? src.ServerPort : 0);

        TypeAdapterConfig<User, CacheUser>.NewConfig().Map(cache => cache.Roles, user => user.Roles.Select(role => role.RoleId).ToList());
        TypeAdapterConfig<User, UserModel>.NewConfig().Map(user => user.Roles, user => user.Roles.Select(ur => new RoleModel
        {
            Id = ur.RoleId,
            Code = ur.Role == null ? "" : ur.Role.Code,
            Name = ur.Role == null ? "" : ur.Role.Name,
        }).ToList());

        TypeAdapterConfig<ThirdPartyIdp, ThirdPartyIdpModel>.ForType()
            .Map(item => item.JsonKeyMap, item => JsonSerializer.Deserialize<Dictionary<string, string>>(string.IsNullOrEmpty(item.JsonKeyMap) ? "{}" : item.JsonKeyMap, new JsonSerializerOptions()));
        TypeAdapterConfig<ThirdPartyIdp, ThirdPartyIdpDetailDto>.ForType()
            .Map(item => item.JsonKeyMap, item => JsonSerializer.Deserialize<Dictionary<string, string>>(string.IsNullOrEmpty(item.JsonKeyMap) ? "{}" : item.JsonKeyMap, new JsonSerializerOptions()));

        TypeAdapterConfig<AuthenticationDefaults, ThirdPartyIdpModel>.ForType()
            .Map(item => item.Name, item => item.Scheme);

        TypeAdapterConfig<Permission, CachePermission>.NewConfig().Map(cache => cache.ApiPermissions, permission => permission.AffiliationPermissionRelations.Select(p => p.AffiliationPermissionId));

        TypeAdapterConfig<DynamicRoleUpsertDto, DynamicRole>.NewConfig()
            .MapToConstructor(true)
            .Map(dest => dest.Conditions, src =>
                src.Conditions.Select((dto, index) => new DynamicRuleCondition(
                    dto.LogicalOperator,
                    dto.FieldName,
                    dto.OperatorType,
                    dto.Value,
                    dto.DataType,
                    index)).ToList());
        TypeAdapterConfig<DynamicRuleConditionUpsertDto, DynamicRuleCondition>.NewConfig().MapToConstructor(true);
    }
}
