// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global;

public class EnvironmentDataService
{
    private string _environment = "";

    public string Environment
    {
        get => _environment;
        set
        {
            _environment = value;
            EnvironmentChanged?.Invoke(this, new EnvironmentDataEventArgs(value));
        }
    }

    public event EventHandler<EnvironmentDataEventArgs>? EnvironmentChanged;
}

public class EnvironmentDataEventArgs : EventArgs
{
    public EnvironmentDataEventArgs(string newValue)
    {
        Value = newValue;
    }

    public string Value { get; }
}