// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class TypeDescriptor
{
    public static Delegate ConvertToDelegateType(MethodInfo method, object target)
    {
        Type delegateType = typeof(Action);
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
        if (method.ReturnType == typeof(void))
        {
            var paramCount = parameterTypes.Count;
            if (paramCount == 0) delegateType = typeof(Action);
            else if (paramCount == 1) delegateType = typeof(Action<>);
            else if (paramCount == 2) delegateType = typeof(Action<,>);
            else if (paramCount == 3) delegateType = typeof(Action<,,>);
            else if (paramCount == 4) delegateType = typeof(Action<,,,>);
            else if (paramCount == 5) delegateType = typeof(Action<,,,,>);
            else if (paramCount == 6) delegateType = typeof(Action<,,,,,>);
            else if (paramCount == 7) delegateType = typeof(Action<,,,,,,>);
            else if (paramCount == 8) delegateType = typeof(Action<,,,,,,,>);
            else if (paramCount == 9) delegateType = typeof(Action<,,,,,,,,>);
        }
        else
        {
            parameterTypes.Add(method.ReturnType);
            var paramCount = parameterTypes.Count;
            if (paramCount == 1) delegateType = typeof(Func<>);
            else if (paramCount == 2) delegateType = typeof(Func<,>);
            else if (paramCount == 3) delegateType = typeof(Func<,,>);
            else if (paramCount == 4) delegateType = typeof(Func<,,,>);
            else if (paramCount == 5) delegateType = typeof(Func<,,,,>);
            else if (paramCount == 6) delegateType = typeof(Func<,,,,,>);
            else if (paramCount == 7) delegateType = typeof(Func<,,,,,,>);
            else if (paramCount == 8) delegateType = typeof(Func<,,,,,,,>);
            else if (paramCount == 9) delegateType = typeof(Func<,,,,,,,,>);
            else if (paramCount == 9) delegateType = typeof(Func<,,,,,,,,,>);
        }
        delegateType = delegateType.MakeGenericType(parameterTypes.ToArray());

        return method.CreateDelegate(delegateType, target);
    }
}

