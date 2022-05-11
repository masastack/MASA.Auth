// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class PaginationDto<TEntity> where TEntity : class
{
    public long Total { get; }

    public List<TEntity> Items { get; }

    public PaginationDto()
    {
        Items = new List<TEntity>();
    }

    [JsonConstructor]
    public PaginationDto(long total, List<TEntity> items)
    {
        Total = total;
        Items = items;
    }
}
