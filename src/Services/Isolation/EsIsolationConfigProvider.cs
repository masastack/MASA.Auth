// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Isolation;

namespace Isolation;

internal class EsIsolationConfigProvider
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    Dictionary<string, object> _esOptions = new();

    public EsIsolationConfigProvider(IMultiEnvironmentContext multiEnvironmentContext)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
    }


}
