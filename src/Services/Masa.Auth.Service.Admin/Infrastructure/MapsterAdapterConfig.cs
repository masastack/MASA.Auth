// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public static class MapsterAdapterConfig
{
    public static void TypeAdapter()
    {
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

        TypeAdapterConfig<ClientPropertyDto, ClientProperty>.NewConfig().MapToConstructor(true);
        TypeAdapterConfig<ClientModel, Client>.NewConfig().MapToConstructor(true);
        TypeAdapterConfig<AddClientDto, Client>.NewConfig().MapToConstructor(true);

        TypeAdapterConfig<LdapDetailDto, LdapOptions>.ForType()
            .Map(dest => dest.ServerPort, src => src.IsLdaps ? 0 : src.ServerPort)
            .Map(dest => dest.ServerPortSsl, src => src.IsLdaps ? src.ServerPort : 0);

        TypeAdapterConfig<AddUserDto, CacheUser>.NewConfig().Map(dest => dest.Permissions, src => src.Permissions.Select(p => p.PermissionId).ToList());
    }
}
