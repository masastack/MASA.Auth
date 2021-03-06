// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoComplete(this IServiceCollection services)
        {
            services.AddElasticsearchClient("auth", option => option.UseNodes("http://10.10.90.44:31920/").UseDefault())
                    .AddAutoComplete<UserSelectDto, Guid>(option => option.UseIndexName("user_index"));

            return services;
        }
    }
}
