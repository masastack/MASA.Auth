// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Psso;

public class AbpApiResponse<T>
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public T? Result { get; set; }
}
