// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public interface ITokenProvider
{
    public string? AccessToken { get; }

    public string? RefreshToken { get; }

    public string? IdToken { get; }
}
