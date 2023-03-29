// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class ElasticsearchAutoCompleteExtensions
{
    public static IServiceCollection AddElasticsearchAutoComplete(this IServiceCollection services)
    {
        var masaStackConfig = services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();

        services.AddAutoCompleteBySpecifyDocument<UserSelectDto>("es", autoCompleteOptions =>
        {
            autoCompleteOptions.UseElasticSearch(options =>
            {
                options.ElasticsearchOptions.UseNodes(masaStackConfig.ElasticModel.Nodes.ToArray())
                    .UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
                options.IndexName = masaStackConfig.ElasticModel.Index;
                //options.Alias = masaStackConfig.ElasticModel.Alias;
            });
        });

        var autoCompleteFactory = services.BuildServiceProvider().GetRequiredService<IAutoCompleteFactory>();
        var autoCompleteClient = autoCompleteFactory.Create();
        autoCompleteClient.BuildAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        return services;
    }
}
