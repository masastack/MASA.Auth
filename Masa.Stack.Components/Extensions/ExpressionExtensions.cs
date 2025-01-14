namespace Masa.Stack.Components;

internal static class ExpressionExtensions
{
    public static string GetFieldName<TValue>(this Expression<Func<TValue>> valueExpression)
    {
        var accessorBody = valueExpression.Body;

        if (accessorBody is UnaryExpression unaryExpression
            && unaryExpression.NodeType == ExpressionType.Convert
            && unaryExpression.Type == typeof(object))
        {
            accessorBody = unaryExpression.Operand;
        }

        return (accessorBody as MemberExpression)!.Member.Name;
    }
}