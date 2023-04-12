// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Isolation;

internal class EsIsolationConfigProvider
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    readonly EnvironmentProvider _environmentProvider;
    readonly IMasaStackConfig _masaStackConfig;
    readonly ILogger<EsIsolationConfigProvider>? _logger;

    Dictionary<string, ElasticModel> _esOptions = new();

    public EsIsolationConfigProvider(
        IMultiEnvironmentContext multiEnvironmentContext,
        EnvironmentProvider environmentProvider,
        IMasaStackConfig masaStackConfig,
        ILogger<EsIsolationConfigProvider>? logger)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
        _environmentProvider = environmentProvider;
        _masaStackConfig = masaStackConfig;
        _logger = logger;

        InitData();
    }

    void InitData()
    {
        foreach (var envionment in _environmentProvider.GetEnvionments())
        {
            var result = _esOptions.TryAdd(envionment, _masaStackConfig.ElasticModel);
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
