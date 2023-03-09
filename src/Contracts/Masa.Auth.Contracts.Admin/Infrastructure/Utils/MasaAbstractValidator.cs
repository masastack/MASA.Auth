// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class MasaAbstractValidator<T> : AbstractValidator<T>
{
    public IConditionBuilder WhenNotNullOrEmpty<TProperty>(Expression<Func<T, TProperty?>> selector, Action<IRuleBuilderInitial<T, TProperty>> action) where TProperty : class
    {
        var selectorFunc = selector.Compile();
        return When(value =>
        {
            var property = selectorFunc(value);
            if (property is string str) return string.IsNullOrEmpty(str) is false;
            else return property is not null;
        }, () => action.Invoke(RuleFor(selector)!));
    }
}
