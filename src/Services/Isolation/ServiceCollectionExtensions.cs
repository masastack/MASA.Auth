// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Isolation;

public static class ServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddStackIsolationAsync(this IServiceCollection services, string name)
    {
        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        var environments = (await pmClient.EnvironmentService.GetListAsync()).Select(e => e.Name).ToList();

        services.AddSingleton((sp) => { return new EnvironmentProvider(environments); });

        ConfigureConnectionStrings(services, name);
        ConfigureRedisOptions(services);
        ConfigStorageOptions(services);

        services.AddSingleton<EsIsolationConfigProvider>();

        services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiEnvironment();
        });

        return services;
    }

    static void ConfigureConnectionStrings(this IServiceCollection services, string name)
    {
        var (environments, multiEnvironmentMasaStackConfig, masaStackConfig) = services.GetInternal();
        services.Configure<IsolationOptions<ConnectionStrings>>(options =>
        {
            foreach (string environment in environments)
            {
                options.Data.Add(new()
                {
                    Environment = environment,
                    Data = new ConnectionStrings(new List<KeyValuePair<string, string>>()
                    {
                        new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, multiEnvironmentMasaStackConfig.SetEnvironment(environment).GetConnectionString(name))
                    })
                });
            }
        });

        services.Configure<ConnectionStrings>(options => new List<KeyValuePair<string, string>>()
                    {
                        new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, masaStackConfig.GetConnectionString(name))
                    });
    }

    static void ConfigureRedisOptions(this IServiceCollection services)
    {
        var (environments, multiEnvironmentMasaStackConfig, masaStackConfig) = services.GetInternal();
        services.Configure<IsolationOptions<RedisConfigurationOptions>>(options =>
        {
            foreach (string environment in environments)
            {
                var redisModel = multiEnvironmentMasaStackConfig.SetEnvironment(environment).RedisModel;
                options.Data.Add(new IsolationConfigurationOptions<RedisConfigurationOptions>()
                {
                    Environment = environment,
                    Data = new RedisConfigurationOptions
                    {
                        Password = redisModel.RedisPassword,
                        Servers = new() {
                            new RedisServerOptions
                            {
                                Host = redisModel.RedisHost,
                                Port = redisModel.RedisPort
                            }
                        },
                        DefaultDatabase = redisModel.RedisDb,
                        InstanceId = environment
                    }
                });
            }
        });
        services.Configure<RedisConfigurationOptions>(options =>
        {
            var redisModel = masaStackConfig.RedisModel;
            options.Password = redisModel.RedisPassword;
            options.Servers = new()
            {
                new RedisServerOptions
                {
                    Host = redisModel.RedisHost,
                    Port = redisModel.RedisPort
                }
            };
            options.DefaultDatabase = redisModel.RedisDb;
        });
    }

    static void ConfigStorageOptions(this IServiceCollection services)
    {
        var (environments, _, masaStackConfig) = services.GetInternal();
        var configurationApiClient = services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        services.Configure<IsolationOptions<AliyunStorageConfigureOptions>>(async options =>
        {
            foreach (string environment in environments)
            {
                var ossOptions = await configurationApiClient.GetAsync<OssOptions>(environment, "Default", "public-$Config", "$public.OSS", ossOptions =>
                {
                    var item = options.Data.FirstOrDefault(s => s.Environment == environment);
                    if (item != null)
                    {
                        item.Data = Convert(ossOptions);
                    }
                });
                if (ossOptions == null)
                {
                    continue;
                }
                options.Data.Add(new IsolationConfigurationOptions<AliyunStorageConfigureOptions>()
                {
                    Environment = environment,
                    Data = Convert(ossOptions)
                });
            }
        });

        services.Configure<AliyunStorageConfigureOptions>(async options =>
        {
            var ossOptions = await configurationApiClient.GetAsync<OssOptions>(masaStackConfig.Environment, "Default", "public-$Config", "$public.OSS");
            options = Convert(ossOptions);
        });

        AliyunStorageConfigureOptions Convert(OssOptions ossOptions)
        {
            return new AliyunStorageConfigureOptions
            {
                AccessKeyId = ossOptions.AccessId,
                AccessKeySecret = ossOptions.AccessSecret,
                Sts = new AliyunStsOptions
                {
                    RegionId = ossOptions.RegionId
                },
                Storage = new AliyunStorageOptions
                {
                    BucketNames = new BucketNames(new Dictionary<string, string> { { BucketNames.DEFAULT_BUCKET_NAME, ossOptions.Bucket } }),
                    Endpoint = ossOptions.Endpoint,
                    RoleArn = ossOptions.RoleArn,
                    RoleSessionName = ossOptions.RoleSessionName
                }
            };
        }
    }

    static (List<string>, IMultiEnvironmentMasaStackConfig, IMasaStackConfig) GetInternal(this IServiceCollection services)
    {
        var masaStackConfig = services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
        var multiEnvironmentMasaStackConfig = services.BuildServiceProvider().GetRequiredService<IMultiEnvironmentMasaStackConfig>();
        var environmentProvider = services.BuildServiceProvider().GetRequiredService<EnvironmentProvider>();
        return (environmentProvider.GetEnvionments(), multiEnvironmentMasaStackConfig, masaStackConfig);
    }
}