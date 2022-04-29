// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientSecret : Secret
{
    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
