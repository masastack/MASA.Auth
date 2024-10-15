// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class MultilevelCacheExtensions
{
    public static RedisConfigurationOptions? GetMultilevelCacheRedisOptions(this IConfiguration configuration, string clientName)
    {
        var multilevelCacheRedisStr = configuration.GetValue<string>("MULTILEVEL_CACHE_REDIS");

        if (string.IsNullOrEmpty(multilevelCacheRedisStr))
        {
            return null;
        }

        var multilevelCacheRedis = JsonSerializer.Deserialize<RedisModel>(multilevelCacheRedisStr) ?? throw new JsonException();
        var redisOption = new RedisConfigurationOptions
        {
            Servers = new List<RedisServerOptions>
                {
                    new RedisServerOptions()
                    {
                        Host= multilevelCacheRedis.RedisHost,
                        Port= multilevelCacheRedis.RedisPort
                    }
                },
            DefaultDatabase = multilevelCacheRedis.RedisDb,
            Password = multilevelCacheRedis.RedisPassword,
            ClientName = clientName
        };

        return redisOption;
    }
}
