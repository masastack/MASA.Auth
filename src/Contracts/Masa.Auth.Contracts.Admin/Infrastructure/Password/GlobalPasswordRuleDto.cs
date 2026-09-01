// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Password;

/// <summary>
/// Represents the global password rule configured in DCC.
/// When a client does not configure its own rule, this global rule is used as the fallback.
/// </summary>
public class GlobalPasswordRuleDto
{
    public string PasswordRule { get; set; } = string.Empty;

    public string PasswordPrompt { get; set; } = string.Empty;
}
