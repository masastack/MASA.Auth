// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class SelectItemDto<T>
{
    public T Value { get; set; } = default!;

    public string Text { get; set; } = string.Empty;

    public SelectItemDto()
    {

    }

    public SelectItemDto(T value, string text)
    {
        Value = value;
        Text = text;
    }
}
