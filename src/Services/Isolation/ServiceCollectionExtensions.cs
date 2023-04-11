// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Isolation;
using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace Isolation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackIsolation(this IServiceCollection services)
    {

        ConfigureConnectionStrings(services);
        ConfigureRedisOptions(services);
        //ConfigStorageOptions(services);

        services.AddSingleton<EsIsolationConfigProvider>();

        services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiEnvironment();
        });

        return services;
    }

    static void ConfigureConnectionStrings(this IServiceCollection services)
    {
        services.Configure<IsolationOptions<ConnectionStrings>>(options =>
        {
            options.Data = new List<IsolationConfigurationOptions<ConnectionStrings>>()
            {
                new()
                {
                    Environment = "租户1",
                    Data = new ConnectionStrings(new List<KeyValuePair<string, string>>()
                    {
                        new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "租户1数据库连接字符串地址")
                    })
                },
                new()
                {
                    Environment = "租户2",
                    Data = new ConnectionStrings(new List<KeyValuePair<string, string>>()
                    {
                        new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "租户2数据库连接字符串地址")
                    })
                },
            };
        });
    }

    static void ConfigureRedisOptions(this IServiceCollection services)
    {
        services.Configure<IsolationOptions<RedisConfigurationOptions>>(options =>
        {
            options.Data = new List<IsolationConfigurationOptions<RedisConfigurationOptions>>()
            {
                new IsolationConfigurationOptions<RedisConfigurationOptions>()
                {
                    Environment = "租户1",
                    Data = new RedisConfigurationOptions
                    {
                        Password = "password",
                        Servers = new(),
                        DefaultDatabase=1,
                        InstanceId = "1"
                    }
                }
            };
        });
    }

    //static void ConfigStorageOptions(this IServiceCollection services)
    //{
    //    services.Configure<IsolationOptions<AliyunStorageConfigureOptions>>(options =>
    //    {
    //        options.Data = new List<IsolationConfigurationOptions<AliyunStorageConfigureOptions>>()
    //        {
    //            new IsolationConfigurationOptions<AliyunStorageConfigureOptions>()
    //            {
    //                Environment = "租户1",
    //                Data = new AliyunStorageConfigureOptions
    //                {

    //                }
    //            }
    //        };
    //    });
    //}
}