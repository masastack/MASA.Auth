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
    readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;

    public CodeAuthorizationMiddlewareResultHandler(
        IMasaAuthorizeDataProvider masaAuthorizeDataProvider,
        ILogger<CodeAuthorizationMiddlewareResultHandler> logger,
        IMultiEnvironmentMasaStackConfig multiEnvironmentMasaStackConfig,
        IMultiEnvironmentUserContext multiEnvironmentUserContext)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
        _logger = logger;
        _multiEnvironmentMasaStackConfig = multiEnvironmentMasaStackConfig;
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        //Enhance the default challenge or forbid responses.
        var endpoint = context.GetEndpoint();
        var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();

        if (masaAuthorizeAttribute == null && context.User.Identity?.IsAuthenticated is true)
        {
            await next(context);
            return;
        }

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
        var requirement = policy.Requirements.Where(r => r is DefaultRuleCodeRequirement)
                .Select(r => r as DefaultRuleCodeRequirement).FirstOrDefault();
        if (requirement != null)
        {
            appId = _multiEnvironmentMasaStackConfig.SetEnvironment(_multiEnvironmentUserContext.Environment ?? "").GetServiceId(requirement.Project);
        }
        if (string.IsNullOrWhiteSpace(code))
        {
            //dafault code rule
            code = Regex.Replace(context.Request.Path, @"\\", ".");
            code = Regex.Replace(code, "/", ".").Trim('.').ToLower();
            code = $"{appId}.{code}";
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
