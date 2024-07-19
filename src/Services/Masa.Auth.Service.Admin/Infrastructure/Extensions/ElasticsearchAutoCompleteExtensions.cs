// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class ElasticsearchAutoCompleteExtensions
{
    static List<string> _indexes = new List<string>();

    public static IServiceCollection AddElasticsearchAutoComplete(this IServiceCollection services)
    {
        services.AddAutoCompleteBySpecifyDocument<UserSelectDto>("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                var esIsolationConfigProvider = services.BuildServiceProvider().GetService<IHttpContextAccessor>()
                    ?.HttpContext?.RequestServices.GetService<EsIsolationConfigProvider>();
                if (esIsolationConfigProvider == null)
                {
                    esIsolationConfigProvider = services.BuildServiceProvider().GetRequiredService<EsIsolationConfigProvider>();
                }
                var esOptions = esIsolationConfigProvider.GetEsOptions();
                options.ElasticsearchOptions.UseNodes(esOptions.Nodes.ToArray())
                    .UseConnectionSettings(setting =>
                    {
                        setting.EnableApiVersioningHeader(false);
                        if (!string.IsNullOrEmpty(esOptions.UserName) && !string.IsNullOrEmpty(esOptions.Password))
                            setting.BasicAuthentication(esOptions.UserName, esOptions.Password);
                    });
                options.IndexName = esOptions.Index;

                if (!_indexes.Contains(esOptions.Index))
                {
                    _indexes.Add(esOptions.Index);
                    var autoCompleteFactory = services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
                    var autoCompleteClient = autoCompleteFactory.Create();
                    autoCompleteClient.BuildAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            });
        });

        return services;
    }
}
