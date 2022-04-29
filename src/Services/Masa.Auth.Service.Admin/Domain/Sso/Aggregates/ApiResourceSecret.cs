// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceSecret : Secret
{
    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;
}

