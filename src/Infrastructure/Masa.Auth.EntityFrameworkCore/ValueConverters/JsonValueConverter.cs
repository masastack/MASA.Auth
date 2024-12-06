// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.ValueConverters;

public class JsonValueConverter<T> : ValueConverter<T, string> where T : class, new()
{
    public JsonValueConverter()
        : base(x => SerializeObject(x), x => DeserializeObject(x))
    {

    }

    private static string SerializeObject(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    private static T DeserializeObject(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return new T();
        }

        return JsonSerializer.Deserialize<T>(json)!;
    }
}
