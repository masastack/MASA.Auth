// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Environment;

public class SsoEnvironmentProvider : ISsoEnvironmentProvider
{
    AsyncLocal<string> _environment = new AsyncLocal<string>();

    public string GetEnvironment() => _environment.Value ?? "";

    public void SetEnvironment(string env)
    {
        _environment.Value = env;
    }
}
