// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Isolation;

public class EsIsolationConfigProvider
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    readonly EnvironmentProvider _environmentProvider;
    readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;
    readonly ILogger<EsIsolationConfigProvider>? _logger;

    Dictionary<string, ElasticModel> _esOptions = new();

    public EsIsolationConfigProvider(
        EnvironmentProvider environmentProvider,
        IServiceProvider serviceProvider,
        ILogger<EsIsolationConfigProvider>? logger)
    {
        _multiEnvironmentContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMultiEnvironmentContext>();
        _environmentProvider = environmentProvider;
        _multiEnvironmentMasaStackConfig = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IMultiEnvironmentMasaStackConfig>();
        _logger = logger;

        InitData();
    }

    void InitData()
    {
        foreach (var envionment in _environmentProvider.GetEnvionments())
        {
            var result = _esOptions.TryAdd(envionment, _multiEnvironmentMasaStackConfig.SetEnvironment(envionment).ElasticModel);
            if (!result)
            {
                _logger?.LogWarning($"Duplicate key {envionment}");
            }
        }
    }

    public ElasticModel GetEsOptions()
    {
        return _esOptions[_multiEnvironmentContext.CurrentEnvironment];
    }
}
