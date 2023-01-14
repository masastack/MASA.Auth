// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class ElasticsearchAutoCompleteExtensions
{
    public static IServiceCollection AddElasticsearchAutoComplete(this IServiceCollection services)
    {
        var masaStackConfig = services.BuildServiceProvider()
                              .GetRequiredService<IMasaStackConfig>();

        var esBuilder = services.AddElasticsearchClient(
                "",
                option => option.UseNodes(masaStackConfig.ElasticModel.ESNode)
            );

        esBuilder.AddAutoCompleteBySpecifyDocument<UserSelectDto>(option =>
        {
            option.UseIndexName(masaStackConfig.ElasticModel.Index);
            //if (string.IsNullOrEmpty(masaStackConfig.ElasticModel.Alias) is false)
            //{
            //    option.UseAlias(masaStackConfig.ElasticModel.Alias);
            //}
        });

        return services;
    }
}
