// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class MapJsonKey
{    
    public List<KeyValue> InternalValue { get; set; } = new();

    [Parameter]
    public Dictionary<string,string> Value
    {
        get => InternalValue.ToDictionary(item => item.Key, item => item.Value);
        set => InternalValue = value.Select(item => new KeyValue
        {
            Key = item.Key,
            Value = item.Value
        }).ToList();
    }

    [Parameter]
    public EventCallback<Dictionary<string, string>> ValueChanged { get; set; }

    public async Task RemoveAsync(KeyValue keyVlaue)
    {
        InternalValue.Remove(keyVlaue);
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(Value);
    }

    public async Task AddAsync()
    {
        InternalValue.Add(new KeyValue());
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(Value);
    }
}
