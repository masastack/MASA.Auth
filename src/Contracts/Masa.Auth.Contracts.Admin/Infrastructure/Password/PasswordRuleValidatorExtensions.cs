// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Password;

/// <summary>
/// Wires <see cref="IPasswordRuleProvider"/> into a FluentValidation rule chain. The configured
/// rule (global DCC default, or a client override) is the single source of truth for what a valid
/// password looks like - no hardcoded required/length checks are layered on top of it here.
/// </summary>
public static class PasswordRuleValidatorExtensions
{
    public static IRuleBuilderOptionsConditions<T, string?> PasswordRule<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        IPasswordRuleProvider passwordRuleProvider,
        Func<T, string?> clientId)
    {
        return ruleBuilder.CustomAsync(async (password, context, cancellation) =>
        {
            var failure = await passwordRuleProvider.GetFailureAsync(password, clientId(context.InstanceToValidate));
            if (failure is not null)
            {
                context.AddFailure(failure);
            }
        });
    }
}
