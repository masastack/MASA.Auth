// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class JwtTokenValidatorOptions
{
    public string AuthorityEndpoint { get; set; } = string.Empty;

    public bool ValidateLifetime { get; set; } = true;

    public bool ValidateAudience { get; set; }

    public bool ValidateIssuer { get; set; } = true;

    public IEnumerable<string> ValidAudiences { get; set; } = Enumerable.Empty<string>();
}
