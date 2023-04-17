// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class BaseUpsertDto<T> where T : IComparable
{
    public T Id { get; set; } = default!;

    public BaseUpsertDto()
    {
        IsUpdate = !Id.Equals(default(T));
    }

    public virtual void SetIsUpdate(bool isUpdate)
    {
        IsUpdate = isUpdate;
    }

    [JsonIgnore]
    public virtual bool IsUpdate { get; private set; }
}
