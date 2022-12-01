// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

internal class Decorator<TService>
{
    public TService Instance { get; set; }

    public Decorator(TService instance)
    {
        Instance = instance;
    }
}

internal class Decorator<TService, TImpl> : Decorator<TService>
    where TImpl : class, TService
{
    public Decorator(TImpl instance) : base(instance)
    {
    }
}

internal class DisposableDecorator<TService> : Decorator<TService>, IDisposable
{
    public DisposableDecorator(TService instance) : base(instance)
    {
    }

    public void Dispose()
    {
        (Instance as IDisposable)?.Dispose();
    }
}
