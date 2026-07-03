// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class PasswordValidator : MasaAbstractValidator<string?>
{
    public PasswordValidator(IPasswordRuleProvider passwordRuleProvider)
    {
        RuleFor(password => password).Required()
            .CustomAsync(async (password, context, cancellation) =>
            {
                var failure = await passwordRuleProvider.GetFailureAsync(password, null);
                if (failure is not null)
                {
                    context.AddFailure(failure);
                }
            });
    }
}
