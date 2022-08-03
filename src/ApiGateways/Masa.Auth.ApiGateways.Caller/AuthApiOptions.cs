// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class AuthApiOptions
{
    public string AuthServiceBaseAddress { get; set; }

    public AuthApiOptions(string authServiceBaseAddress)
    {
        AuthServiceBaseAddress = authServiceBaseAddress;
    }
}
