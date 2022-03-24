using System.Linq.Expressions;

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class CustomizeAbstractValidator<T> : AbstractValidator<T>
{
    public new CustomizeRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var memberExpression = (MemberExpression)expression.Body;

        return new CustomizeRuleBuilderInitial<T, TProperty>(memberExpression.Member.Name, base.RuleFor(expression));
    }
}

public class CustomizeRuleBuilderInitial<T, TProperty>
{
    public string PropertyName { get; init; }

    public IRuleBuilder<T, TProperty> Rule { get; init; }

    public CustomizeRuleBuilderInitial(string propertyName, IRuleBuilderInitial<T, TProperty> rule)
    {
        PropertyName = propertyName;
        Rule = rule;
        Rule.NotNull().WithMessage($"Parameter error");
    }
}

