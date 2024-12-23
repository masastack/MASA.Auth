// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientCredentialDto
{
    public bool RequireClientSecret { get; set; }

    public List<ClientSecretDto> ClientSecrets { get; set; } = new();

    public ClientSecretDto ClientSecret { get; set; } = new();
}
