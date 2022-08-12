// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Extensions;

public static class ElasticsearchAutoCompleteExtensions
{
    public static void AddElasticsearchAutoComplete(this IServiceCollection services)
    {
        var options = services.BuildServiceProvider().GetService<IOptions<AutoCompleteModel>>();
        var autoCompleteModel = options!.Value;

        var esBuilder = services.AddElasticsearchClient(autoCompleteModel.Name, option => option.UseNodes(autoCompleteModel.Nodes.ToArray())
                .UseDefault());
        foreach (var doc in autoCompleteModel.Documents)
        {
            esBuilder.AddAutoComplete<UserSelectDto, Guid>(option => 
            {
                option.UseIndexName(doc.Index);
                if (string.IsNullOrEmpty(doc.Alias) is false) option.UseAlias(doc.Alias);
                else option.UseAlias(doc.Index);
            });
        }
    }
}