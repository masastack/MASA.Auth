// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso;

internal static class ClientPasswordRuleRegexConverter
{
    private static readonly Regex ConfigRuleRegex = new(
        @"^\^\(\?=\\S\{(?<min>\d+),(?<max>\d+)\}\$\)(?<checks>(?:\(\?=\\S\*\[A-Z\]\)|\(\?=\\S\*\[a-z\]\)|\(\?=\\S\*\\d\)|\(\?=\\S\*\[\^A-Za-z0-9\\s\]\))*)\\S\*\$$",
        RegexOptions.Compiled);

    public static ClientPasswordRuleDto Normalize(ClientPasswordRuleDto? passwordRule)
    {
        if (passwordRule is null)
        {
            passwordRule = new();
        }

        passwordRule.MinLength = Math.Max(1, passwordRule.MinLength);
        passwordRule.MaxLength = Math.Max(passwordRule.MinLength, passwordRule.MaxLength);
        passwordRule.RegexPattern = passwordRule.RegexPattern?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(passwordRule.PasswordPrompt))
        {
            passwordRule.PasswordPrompt = BuildPasswordPrompt(passwordRule);
        }
        else
        {
            passwordRule.PasswordPrompt = passwordRule.PasswordPrompt.Trim();
        }

        return passwordRule;
    }

    public static string? ToRegex(ClientPasswordRuleDto? passwordRule)
    {
        passwordRule = Normalize(passwordRule);

        if (passwordRule.UseRegexPattern)
        {
            return string.IsNullOrEmpty(passwordRule.RegexPattern)
                ? null
                : passwordRule.RegexPattern;
        }

        var minLength = Math.Max(1, passwordRule.MinLength);
        var maxLength = Math.Max(minLength, passwordRule.MaxLength);
        var lookaheads = new List<string>();

        if (passwordRule.RequireUppercase)
        {
            lookaheads.Add(@"(?=\S*[A-Z])");
        }

        if (passwordRule.RequireLowercase)
        {
            lookaheads.Add(@"(?=\S*[a-z])");
        }

        if (passwordRule.RequireDigit)
        {
            lookaheads.Add(@"(?=\S*\d)");
        }

        if (passwordRule.RequireSpecialCharacter)
        {
            lookaheads.Add(@"(?=\S*[^A-Za-z0-9\s])");
        }

        return $@"^(?=\S{{{minLength},{maxLength}}}$){string.Join(string.Empty, lookaheads)}\S*$";
    }

    public static ClientPasswordRuleConfig ToDomainConfig(ClientPasswordRuleDto? passwordRule)
    {
        passwordRule = Normalize(passwordRule);

        return new ClientPasswordRuleConfig
        {
            MinLength = passwordRule.MinLength,
            MaxLength = passwordRule.MaxLength,
            RequireUppercase = passwordRule.RequireUppercase,
            RequireLowercase = passwordRule.RequireLowercase,
            RequireDigit = passwordRule.RequireDigit,
            RequireSpecialCharacter = passwordRule.RequireSpecialCharacter,
            UseRegexPattern = passwordRule.UseRegexPattern,
            RegexPattern = passwordRule.RegexPattern,
            PasswordPrompt = passwordRule.PasswordPrompt
        };
    }

    public static ClientPasswordRuleDto FromStorage(ClientPasswordRuleConfig? passwordRuleConfig, string? regexPattern, string? passwordPrompt)
    {
        if (passwordRuleConfig is not null)
        {
            return Normalize(new ClientPasswordRuleDto
            {
                MinLength = passwordRuleConfig.MinLength,
                MaxLength = passwordRuleConfig.MaxLength,
                RequireUppercase = passwordRuleConfig.RequireUppercase,
                RequireLowercase = passwordRuleConfig.RequireLowercase,
                RequireDigit = passwordRuleConfig.RequireDigit,
                RequireSpecialCharacter = passwordRuleConfig.RequireSpecialCharacter,
                UseRegexPattern = passwordRuleConfig.UseRegexPattern,
                RegexPattern = passwordRuleConfig.RegexPattern,
                PasswordPrompt = string.IsNullOrWhiteSpace(passwordRuleConfig.PasswordPrompt)
                    ? passwordPrompt
                    : passwordRuleConfig.PasswordPrompt
            });
        }

        var legacyPasswordRule = FromRegex(regexPattern);
        legacyPasswordRule.PasswordPrompt = string.IsNullOrWhiteSpace(passwordPrompt)
            ? legacyPasswordRule.PasswordPrompt
            : passwordPrompt;
        return Normalize(legacyPasswordRule);
    }

    private static ClientPasswordRuleDto FromRegex(string? regexPattern)
    {
        if (string.IsNullOrWhiteSpace(regexPattern))
        {
            return new();
        }

        var match = ConfigRuleRegex.Match(regexPattern);
        if (!match.Success)
        {
            return new ClientPasswordRuleDto
            {
                UseRegexPattern = true,
                RegexPattern = regexPattern
            };
        }

        var checks = match.Groups["checks"].Value;
        return new ClientPasswordRuleDto
        {
            MinLength = int.Parse(match.Groups["min"].Value),
            MaxLength = int.Parse(match.Groups["max"].Value),
            RequireUppercase = checks.Contains(@"(?=\S*[A-Z])"),
            RequireLowercase = checks.Contains(@"(?=\S*[a-z])"),
            RequireDigit = checks.Contains(@"(?=\S*\d)"),
            RequireSpecialCharacter = checks.Contains(@"(?=\S*[^A-Za-z0-9\s])")
        };
    }

    private static string BuildPasswordPrompt(ClientPasswordRuleDto passwordRule)
    {
        if (passwordRule.UseRegexPattern)
        {
            return "PasswordValidateFailed";
        }

        var requirements = new List<string>
        {
            $"Password length must be between {passwordRule.MinLength} and {passwordRule.MaxLength} characters"
        };

        if (passwordRule.RequireUppercase)
        {
            requirements.Add("include an uppercase letter");
        }

        if (passwordRule.RequireLowercase)
        {
            requirements.Add("include a lowercase letter");
        }

        if (passwordRule.RequireDigit)
        {
            requirements.Add("include a digit");
        }

        if (passwordRule.RequireSpecialCharacter)
        {
            requirements.Add("include a special character");
        }

        return string.Join(", ", requirements);
    }
}