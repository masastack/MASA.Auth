// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class CustomOptionsMonitor<TOption> : IOptionsMonitor<TOption> where TOption : class, new()
{
    readonly TOption _option;
    readonly IEnumerable<IPostConfigureOptions<TOption>> _configs;

    public CustomOptionsMonitor(IEnumerable<IPostConfigureOptions<TOption>> configs)
    {
        _option = new();
        _configs = configs;
        foreach (var config in configs)
        {
            config.PostConfigure(Options.DefaultName, _option);
        }
    }

    public TOption CurrentValue => _option;

    public TOption Get(string name)
    {
        return _option;
    }

    public IDisposable OnChange(Action<TOption, string> listener)
    {
        throw new NotImplementedException();
    }
}
