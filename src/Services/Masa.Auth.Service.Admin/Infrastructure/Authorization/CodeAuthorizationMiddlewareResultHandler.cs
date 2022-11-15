// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

/// <summary>
/// all route with delegate use Authorize Attribute into this,and will Authorization code
/// </summary>
public class CodeAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
    readonly IMasaAuthorizeDataProvider _masaAuthorizeDataProvider;
    readonly ILogger<CodeAuthorizationMiddlewareResultHandler> _logger;

    public CodeAuthorizationMiddlewareResultHandler(IMasaAuthorizeDataProvider masaAuthorizeDataProvider, ILogger<CodeAuthorizationMiddlewareResultHandler> logger)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
        _logger = logger;
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        //Enhance the default challenge or forbid responses.
        var endpoint = context.GetEndpoint();
        var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
        if (masaAuthorizeAttribute != null)
        {
            if (masaAuthorizeAttribute.Roles?.Split(',').ToList()
                .Intersect(await _masaAuthorizeDataProvider.GetRolesAsync()).Any() == true)
            {
                _logger.LogInformation("----- authentication role passed");
                await next(context);
                return;
            }
        }
        var code = masaAuthorizeAttribute?.Code;
        var appId = string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            //dafault code rule
            code = Regex.Replace(context.Request.Path, @"\\", ".");
            code = Regex.Replace(code, "/", ".").Trim('.').ToLower();
            var requirement = policy.Requirements.Where(r => r is DefaultRuleCodeRequirement)
                .Select(r => r as DefaultRuleCodeRequirement).FirstOrDefault();
            if (requirement != null)
            {
                code = $"{requirement.AppId}.{code}";
                appId = requirement.AppId;
            }
        }

        if (!(await _masaAuthorizeDataProvider.GetAllowCodesAsync(appId)).WildCardContains(code))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            _logger.LogError("authentication failed code {0}", code);
            //Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }
        await next(context);
    }
}
