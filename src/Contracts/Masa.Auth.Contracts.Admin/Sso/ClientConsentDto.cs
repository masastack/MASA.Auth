// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientConsentDto
{
    public int Id { get; set; }

    public string ClientUri { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public bool RequireConsent { get; set; } = true;

    public bool AllowRememberConsent { get; set; }

    public int? ConsentLifetime { get; set; }
}
