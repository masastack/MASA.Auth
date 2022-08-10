// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Environment;

public interface ISsoEnvironmentProvider : IEnvironmentProvider
{
    void SetEnvironment(string env);
}
