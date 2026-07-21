// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Password;

/// <summary>
/// Resolves the effective password rule for a given client and validates passwords against it.
/// A client-level rule (stored on ClientConfig) takes precedence; when absent it falls back to
/// the global DCC configuration.
/// </summary>
public interface IPasswordRuleProvider : IScopedDependency
{
    /// <summary>
    /// Validate a password against the client's effective rule.
    /// Returns the localized failure prompt, or null when the password is valid.
    /// Designed to be called from a FluentValidation rule (see <see cref="PasswordRuleValidatorExtensions"/>)
    /// so client-aware password validation stays inside the single, framework-invoked validation pipeline.
    /// </summary>
    string? GetFailure(string? password, string? clientId);

    Task<string?> GetFailureAsync(string? password, string? clientId);

    /// <summary>
    /// Generate a new password satisfying the global DCC password rule.
    /// </summary>
    Task<string> GenerateNewPasswordAsync();
}
