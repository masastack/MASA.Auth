// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Extensions;

public static class ElasticsearchAutoCompleteExtensions
{
    public static IServiceCollection AddElasticsearchAutoComplete(this IServiceCollection services)
    {
        var options = services.BuildServiceProvider()
                              .GetRequiredService<IOptions<UserAutoCompleteOptions>>()
                              .Value;

        services.AddAutoCompleteBySpecifyDocument<UserSelectDto>("", option =>
        {
            option.UseElasticSearch(esOption =>
            {
                esOption.ElasticsearchOptions.UseNodes(options.Nodes);
                if (string.IsNullOrEmpty(options.Alias) is false)
                {
                    esOption.Alias = options.Alias;
                }
            });
        });

        return services;
    }
}