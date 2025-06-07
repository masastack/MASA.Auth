// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class FromUri<T>
{
    static readonly ConcurrentDictionary<Type, Func<HttpRequest, T>> _map;

    static FromUri()
    {
        _map = new();
    }

    static Guid ToGuid(string guid)
    {
        return Guid.Parse(guid);
    }

    public static ValueTask<T?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var type = typeof(T);
        if (_map.TryGetValue(type, out var func) is false)
        {
            var constructor = type.GetConstructors().First();
            var constructorParamters = constructor.GetParameters().Select(p =>
            {
                if (p.ParameterType.IsValueType)
                {
                    return (CommonValueExpression)Expr.New(p.ParameterType);
                }
                else
                {
                    return (CommonValueExpression)Expr.Constant(null, p.ParameterType);
                }
            });
            Var dto = Expr.New<T>(constructorParamters);
            Var query = new(Expr.BlockParam<HttpRequest>()[nameof(HttpRequest.Query)]);
            var properties = type.GetProperties();
            foreach (var propertie in properties)
            {
                var value = query[$"[{propertie.Name}]"].Method(nameof(StringValues.ToString));
                Expr.IfThen(value != "", () =>
                {
                    dto[propertie.Name] = ConvertValue(value, propertie.PropertyType);
                });
            }
            func = dto.BuildDelegate<Func<HttpRequest, T>>();
            _map[type] = func;

            CommonValueExpression ConvertValue(CommonValueExpression value, Type propertyType)
            {
                if (propertyType == typeof(string)) return value;
                else if (propertyType == typeof(int)) return Expr.Static(typeof(Convert)).Method(nameof(Convert.ToInt32), value);
                else if (propertyType == typeof(long)) return Expr.Static(typeof(Convert)).Method(nameof(Convert.ToInt64), value);
                else if (propertyType == typeof(bool)) return Expr.Static(typeof(Convert)).Method(nameof(Convert.ToBoolean), value);
                else if (propertyType == typeof(Guid)) return Expr.Static<FromUri<string>>().Method(nameof(FromUri<string>.ToGuid), value);
                else if (propertyType.BaseType == typeof(Enum)) return Expr.Static(typeof(Convert)).Method(nameof(Convert.ToInt32), value).Convert(propertyType);
                else if (propertyType == typeof(DateTime)) return Expr.Static(typeof(Convert)).Method(nameof(Convert.ToDateTime), value);
                else if (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) return ConvertValue(value, propertyType.GetGenericArguments()[0]).Convert(propertyType);
                else throw new Exception("This type is not recognized, please add this type detection logic");
            }


        }
        return ValueTask.FromResult<T?>(func(context.Request));
    }
}
