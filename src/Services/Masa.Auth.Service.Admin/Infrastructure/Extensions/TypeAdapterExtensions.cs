// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class TypeAdapterExtensions
{
    public static IServiceCollection AddTypeAdapter(this IServiceCollection services)
    {
        TypeAdapterConfig<ThirdPartyIdp, ThirdPartyIdpModel>.ForType()
            .Map(item => item.JsonKeyMap, item => JsonSerializer.Deserialize<Dictionary<string,string>>(item.JsonKeyMap, new JsonSerializerOptions()));
        TypeAdapterConfig<ThirdPartyIdp, ThirdPartyIdpDetailDto>.ForType()
            .Map(item => item.JsonKeyMap, item => JsonSerializer.Deserialize<Dictionary<string, string>>(item.JsonKeyMap, new JsonSerializerOptions()));

        return services;
    }
}
