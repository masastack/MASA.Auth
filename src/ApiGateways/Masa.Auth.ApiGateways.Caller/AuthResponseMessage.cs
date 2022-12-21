// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class AuthResponseMessage : DefaultResponseMessage
{
    public AuthResponseMessage(IOptions<CallerFactoryOptions> options, ILogger<DefaultResponseMessage>? logger = null) : base(options, logger)
    {
    }

    public override async Task ProcessCustomException(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case (HttpStatusCode)MasaAuthHttpStatusCode.UserStatusException:
                throw new UserStatusException(await response.Content.ReadAsStringAsync());
            default:
                break;
        }
    }
}
