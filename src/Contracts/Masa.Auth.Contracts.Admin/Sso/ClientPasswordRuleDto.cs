// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientPasswordRuleDto
{
    public bool UseRegexPattern { get; set; }

    public int MinLength { get; set; } = 6;

    public int MaxLength { get; set; } = 20;

    public bool RequireUppercase { get; set; }

    public bool RequireLowercase { get; set; }

    public bool RequireDigit { get; set; }

    public bool RequireSpecialCharacter { get; set; }

    public string RegexPattern { get; set; } = string.Empty;

    public string? PasswordPrompt { get; set; }
}
