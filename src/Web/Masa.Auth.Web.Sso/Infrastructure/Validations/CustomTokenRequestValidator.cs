// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class CustomTokenRequestValidator : ICustomTokenRequestValidator
{
    public Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
        context.Result.ValidatedRequest.Client.AlwaysSendClientClaims = true;
        context.Result.CustomResponse =
            new Dictionary<string, object> { { "api_version", "v1.0" } };
        return Task.CompletedTask;
    }
}
