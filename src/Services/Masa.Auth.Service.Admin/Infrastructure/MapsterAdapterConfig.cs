// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public static class MapsterAdapterConfig
{
    public static void TypeAdapter()
    {
        TypeAdapterConfig<string, ClientGrantType>.NewConfig().MapWith(str => new ClientGrantType(str));
        TypeAdapterConfig<string, ClientRedirectUri>.NewConfig().MapWith(str => new ClientRedirectUri(str));
        TypeAdapterConfig<string, ClientPostLogoutRedirectUri>.NewConfig().MapWith(str => new ClientPostLogoutRedirectUri(str));
        TypeAdapterConfig<string, ClientScope>.NewConfig().MapWith(str => new ClientScope(str));
        TypeAdapterConfig<ClientGrantType, string>.NewConfig().MapWith(src => src.GrantType);
        TypeAdapterConfig<ClientRedirectUri, string>.NewConfig().MapWith(src => src.RedirectUri);
        TypeAdapterConfig<ClientPostLogoutRedirectUri, string>.NewConfig().MapWith(src => src.PostLogoutRedirectUri);
        TypeAdapterConfig<ClientScope, string>.NewConfig().MapWith(src => src.Scope);

        TypeAdapterConfig<ClientPropertyDto, ClientProperty>.NewConfig().MapToConstructor(true);

        TypeAdapterConfig<LdapDetailDto, LdapOptions>.ForType()
            .Map(dest => dest.ServerPort, src => src.IsLdaps ? 0 : src.ServerPort)
            .Map(dest => dest.ServerPortSsl, src => src.IsLdaps ? src.ServerPort : 0);
    }
}
