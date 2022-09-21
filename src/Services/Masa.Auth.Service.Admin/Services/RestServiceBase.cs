// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using TypeDescriptor = Masa.Auth.Contracts.Admin.Infrastructure.Utils.TypeDescriptor;

namespace Masa.Auth.Service.Admin.Services;

public abstract class RestServiceBase : ServiceBase
{
    public RestServiceBase(string baseUri) : base(baseUri)
    {
        var type = GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var method in methods)
        {
            if (method.Name.EndsWith("Async"))
            {
                var @delegate = TypeDescriptor.ConvertToDelegateType(method, this);
                if (method.Name.StartsWith("Get")) MapGet(@delegate);
                else if (method.Name.StartsWith("Add")) MapPost(@delegate);
                else if (method.Name.StartsWith("Update")) MapPut(@delegate);
                else if (method.Name.StartsWith("Upsert")) MapPost(@delegate);
                else if (method.Name.StartsWith("Remove")) MapDelete(@delegate);
            }
        }
    }
}

