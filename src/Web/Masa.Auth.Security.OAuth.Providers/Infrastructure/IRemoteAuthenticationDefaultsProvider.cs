﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public interface IRemoteAuthenticationDefaultsProvider
{
    Task<AuthenticationDefaults?> GetAsync(string scheme);

    Task<List<AuthenticationDefaults>> GetAllAsync();
}
