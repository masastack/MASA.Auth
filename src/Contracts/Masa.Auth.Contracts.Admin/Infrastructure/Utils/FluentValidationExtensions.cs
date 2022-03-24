namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class CustomizeFluentValidationExtensions
{
    public static CustomizeRuleBuilderInitial<T, string> Chinese<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.CHINESE)
                        .WithMessage($"Can only input chinese of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> Number<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.NUMBER)
                         .WithMessage($"Can only input number of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> Letter<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.LETTER)
                          .WithMessage($"Can only input letter of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> LowerLetter<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.LOWER_LETTER)
                          .WithMessage($"Can only input lower letter of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> UpperLetter<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.UPPER_LETTER)
                          .WithMessage($"Can only input upper letter of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> LetterNumber<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.LETTER_NUMBER)
                          .WithMessage($"Can only input upper letter and number of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> ChineseLetter<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.CHINESE_LETTER)
                          .WithMessage($"Can only input upper chinese and letter of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> ChineseLetterNumber<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.CHINESE_LETTER_NUMBER)
                         .WithMessage($"Can only input upper chinese and letter and number of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> Phone<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.PHONE)
                         .WithMessage($"{ruleBuilder.PropertyName} format is incorrect");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> Email<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.EMAIL)
                         .WithMessage($"{ruleBuilder.PropertyName} format is incorrect");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> IdCard<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.IDCARD)
                          .WithMessage($"{ruleBuilder.PropertyName} format is incorrect");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> Url<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder)
    {
        ruleBuilder.Rule.Matches(RegularHelper.URL)
                         .WithMessage($"{ruleBuilder.PropertyName} format is incorrect");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> MinimumLength<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder, int minimumLength)
    {
        ruleBuilder.Rule.MinimumLength(minimumLength)
                        .WithMessage($"Please enter a number greater than {minimumLength} of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, string> MaximumLength<T>(this CustomizeRuleBuilderInitial<T, string> ruleBuilder, int maximumLength)
    {
        ruleBuilder.Rule.MaximumLength(maximumLength)
                        .WithMessage($"Please enter a number less than {maximumLength} of {ruleBuilder.PropertyName}");

        return ruleBuilder;
    }

    public static CustomizeRuleBuilderInitial<T, TProperty> Required<T, TProperty>(this CustomizeRuleBuilderInitial<T, TProperty> ruleBuilder)
    {
        ruleBuilder.Rule.NotNull().NotEmpty()
                        .WithMessage($"{ruleBuilder.PropertyName} is required ");

        return ruleBuilder;
    }
}
