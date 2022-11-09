// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Environment;

public class SsoEnvironmentProvider : ISsoEnvironmentProvider
{
    string? _environment;

    public SsoEnvironmentProvider()
    {

    }

    public string GetEnvironment() => _environment ?? "";

    public void SetEnvironment(string env) => _environment = env;
}
