// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class BaseUpsertDto<T> where T : IComparable
{
    public T Id { get; set; } = default!;

    [JsonIgnore]
    public bool IsUpdate => !Id.Equals(default(T));
}
