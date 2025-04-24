// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class OperatorType : Enumeration
{
    public static OperatorType Default = new OperatorType();

    public static OperatorType GreaterThan = new GreaterThanOperator();

    public static OperatorType GreaterThanOrEqual = new GreaterThanOrEqualOperator();

    public static OperatorType LessThan = new LessThanOperator();

    public static OperatorType LessThanOrEqual = new LessThanOrEqualOperator();

    public static OperatorType Equal = new EqualOperator();

    public static OperatorType EqualIgnoreCase = new EqualIgnoreCaseOperator();

    public static OperatorType StartsWith = new StartsWithOperator();

    public static OperatorType NotEndsWith = new NotEndsWithOperator();

    public static OperatorType NotEqual = new NotEqualOperator();

    public static OperatorType NotEqualIgnoreCase = new NotEqualIgnoreCaseOperator();

    public static OperatorType InCollection = new InCollectionOperator();

    public static OperatorType NotInCollection = new NotInCollectionOperator();

    public static OperatorType IsNull = new IsNullOperator();

    public static OperatorType IsNotNull = new IsNotNullOperator();

    public static OperatorType MatchesRegex = new MatchesRegexOperator();

    public static OperatorType NotMatchRegex = new NotMatchRegexOperator();

    public static OperatorType Contains = new ContainsOperator();

    public static OperatorType NotContains = new NotContainsOperator();

    public OperatorType() : base(0, "") { }

    public OperatorType(int id, string name) : base(id, name)
    {
    }

    public virtual bool EvaluateCondition(string? data, string value)
    {
        throw new NotImplementedException();
    }

    public static OperatorType StartNew(string type) => type switch
    {

        nameof(GreaterThan) => new GreaterThanOperator(),
        nameof(GreaterThanOrEqual) => new GreaterThanOrEqualOperator(),
        nameof(LessThan) => new LessThanOperator(),
        nameof(LessThanOrEqual) => new LessThanOrEqualOperator(),
        nameof(Equal) => new EqualOperator(),
        nameof(EqualIgnoreCase) => new EqualIgnoreCaseOperator(),
        nameof(StartsWith) => new StartsWithOperator(),
        nameof(NotEndsWith) => new NotEndsWithOperator(),
        nameof(NotEqual) => new NotEqualOperator(),
        nameof(NotEqualIgnoreCase) => new NotEqualIgnoreCaseOperator(),
        nameof(InCollection) => new InCollectionOperator(),
        nameof(NotInCollection) => new NotInCollectionOperator(),
        nameof(IsNull) => new IsNullOperator(),
        nameof(IsNotNull) => new IsNotNullOperator(),
        nameof(MatchesRegex) => new MatchesRegexOperator(),
        nameof(NotMatchRegex) => new NotMatchRegexOperator(),
        nameof(Contains) => new ContainsOperator(),
        nameof(NotContains) => new NotContainsOperator(),
        _ => new OperatorType()
    };

    private class GreaterThanOperator : OperatorType
    {
        public GreaterThanOperator() : base(1, nameof(OperatorType)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return Convert.ToInt32(data) > Convert.ToInt32(value);
        }
    }

    private class GreaterThanOrEqualOperator : OperatorType
    {
        public GreaterThanOrEqualOperator() : base(2, nameof(GreaterThanOrEqual)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return Convert.ToInt32(data) >= Convert.ToInt32(value);
        }
    }

    private class LessThanOperator : OperatorType
    {
        public LessThanOperator() : base(3, nameof(LessThan)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return Convert.ToInt32(data) < Convert.ToInt32(value);
        }
    }

    private class LessThanOrEqualOperator : OperatorType
    {
        public LessThanOrEqualOperator() : base(4, nameof(LessThanOrEqual)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return Convert.ToInt32(data) <= Convert.ToInt32(value);
        }
    }

    private class EqualOperator : OperatorType
    {
        public EqualOperator() : base(5, nameof(Equal)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return data == value;
        }
    }

    private class EqualIgnoreCaseOperator : OperatorType
    {
        public EqualIgnoreCaseOperator() : base(6, nameof(EqualIgnoreCase)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return string.Equals(data, value, StringComparison.OrdinalIgnoreCase);
        }
    }

    private class StartsWithOperator : OperatorType
    {
        public StartsWithOperator() : base(7, nameof(StartsWith)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return data?.StartsWith(value, StringComparison.Ordinal) == true;
        }
    }

    private class NotEndsWithOperator : OperatorType
    {
        public NotEndsWithOperator() : base(8, nameof(NotEndsWith)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !(data?.EndsWith(value, StringComparison.Ordinal) == true);
        }
    }

    private class NotEqualOperator : OperatorType
    {
        public NotEqualOperator() : base(9, nameof(NotEqual)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !string.Equals(data, value);
        }
    }

    private class NotEqualIgnoreCaseOperator : OperatorType
    {
        public NotEqualIgnoreCaseOperator() : base(10, nameof(NotEqualIgnoreCase)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !string.Equals(data, value, StringComparison.OrdinalIgnoreCase);
        }
    }

    private class InCollectionOperator : OperatorType
    {
        public InCollectionOperator() : base(11, nameof(InCollection)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return (value.Split(";")).Contains(data);
        }
    }

    private class NotInCollectionOperator : OperatorType
    {
        public NotInCollectionOperator() : base(12, nameof(NotInCollection)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !(value.Split(";")).Contains(data);
        }
    }

    private class IsNullOperator : OperatorType
    {
        public IsNullOperator() : base(13, nameof(IsNull)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return data == null;
        }
    }

    private class IsNotNullOperator : OperatorType
    {
        public IsNotNullOperator() : base(14, nameof(IsNotNull)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return data != null;
        }
    }

    private class MatchesRegexOperator : OperatorType
    {
        public MatchesRegexOperator() : base(15, nameof(MatchesRegex)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(data ?? string.Empty, value);
        }
    }

    private class NotMatchRegexOperator : OperatorType
    {
        public NotMatchRegexOperator() : base(16, nameof(NotMatchRegex)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(data ?? string.Empty, value);
        }
    }

    private class ContainsOperator : OperatorType
    {
        public ContainsOperator() : base(17, nameof(Contains)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return data?.Contains(value, StringComparison.Ordinal) == true;
        }
    }

    private class NotContainsOperator : OperatorType
    {
        public NotContainsOperator() : base(18, nameof(NotContains)) { }

        public override bool EvaluateCondition(string? data, string value)
        {
            return !(data?.Contains(value, StringComparison.Ordinal) == true);
        }
    }
}
