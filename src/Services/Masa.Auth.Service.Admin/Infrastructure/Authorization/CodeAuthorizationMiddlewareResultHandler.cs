// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

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
        var allowAnonymousAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (allowAnonymousAttribute == null)
        {
            var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
            if (masaAuthorizeAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(masaAuthorizeAttribute.Account)
                    && !masaAuthorizeAttribute.Account.Equals(_masaAuthorizeDataProvider.GetAccount()))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }
            var code = masaAuthorizeAttribute?.Code;
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
                }
            }
            if (!WildCardContainsCode(_masaAuthorizeDataProvider.GetAllowCodeList(), code))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);

        bool WildCardContainsCode(IEnumerable<string> data, string code)
        {
            return data.Any(item => Regex.IsMatch(code.ToLower(),
                Regex.Escape(item.ToLower()).Replace(@"\*", ".*").Replace(@"\?", ".")));
        }
    }
}
