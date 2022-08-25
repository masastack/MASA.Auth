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

    public CodeAuthorizationMiddlewareResultHandler(IMasaAuthorizeDataProvider masaAuthorizeDataProvider)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        //Enhance the default challenge or forbid responses.
        var endpoint = context.GetEndpoint();
        var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
        if (masaAuthorizeAttribute != null)
        {
            if (!string.IsNullOrWhiteSpace(masaAuthorizeAttribute.Account)
                && !masaAuthorizeAttribute.Account.Equals(await _masaAuthorizeDataProvider.GetAccountAsync()))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
        var code = masaAuthorizeAttribute?.Code;
        var appId = string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            //dafault code rule
            code = Regex.Replace(context.Request.Path, @"\\", ".");
            code = Regex.Replace(code, "/", ".").Trim('.');
            var requirement = policy.Requirements.Where(r => r is DefaultRuleCodeRequirement)
                .Select(r => r as DefaultRuleCodeRequirement).FirstOrDefault();
            if (requirement != null)
            {
                code = $"{requirement.AppId}.{code}";
                appId = requirement.AppId;
            }
        }
        if (!(await _masaAuthorizeDataProvider.GetAllowCodeListAsync(appId)).WildCardContains(code))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
