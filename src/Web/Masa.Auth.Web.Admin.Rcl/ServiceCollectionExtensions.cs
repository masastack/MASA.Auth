// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoComplete(this IServiceCollection services)
        {
            services.AddElasticsearchClient("auth", option => option.UseNodes("http://192.168.10.182:9200").UseDefault())
                    .AddAutoComplete<UserSelectDto, Guid>(option => option.UseIndexName("user_index"));

            return services;
        }
    }
}
