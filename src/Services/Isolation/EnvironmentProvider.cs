﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

public class EnvironmentProvider
{
    List<string> _environments = new();

    public EnvironmentProvider(List<string> environments)
    {
        _environments = environments;
    }

    public List<string> GetEnvionments()
    {
        return _environments;
    }
}