// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientSecretDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
    public DateOnly? Expiration { get; set; }

    public string Type { get; set; } = "SharedSecret";
}
