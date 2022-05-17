// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientSelectDto
{
    public int Id { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public ClientTypes ClientType { get; set; }

    public ClientSelectDto()
    {
    }

    public ClientSelectDto(int id, string clientName, string logoUri, string description, string clientId, ClientTypes clientType)
    {
        Id = id;
        ClientName = clientName;
        LogoUri = logoUri;
        Description = description;
        ClientId = clientId;
        ClientType = clientType;
    }
}
