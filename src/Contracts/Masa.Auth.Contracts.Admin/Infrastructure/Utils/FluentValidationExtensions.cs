// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> RequiredIf<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, bool> condition)
    {
        return ruleBuilder.Must((register, value) =>
                        {
                            if (condition(register))
                            {
                                if (value is string stringValue) return string.IsNullOrEmpty(stringValue) is false;
                                else return value is not null;
                            }
                            return true;
                        })
                        .WithMessage("{PropertyName} is required");
    }
}
