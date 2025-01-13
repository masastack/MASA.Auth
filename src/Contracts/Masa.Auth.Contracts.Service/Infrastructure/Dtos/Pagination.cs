// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class Pagination<T> : FromUri<T>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}


