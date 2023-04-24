// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultRuleCodePolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    readonly IServiceProvider _serviceProvider;

    public DefaultRuleCodePolicyProvider(
        IOptions<AuthorizationOptions> options,
        IServiceProvider serviceProvider)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _serviceProvider = serviceProvider;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        //provider DefaultRuleCode policy or AddAuthorization set default policy
        if (policyName == "DefaultRuleCode")
        {
            var policy = new AuthorizationPolicyBuilder();
            using var scope = _serviceProvider.CreateScope();
            policy.AddRequirements(new DefaultRuleCodeRequirement(scope.ServiceProvider.GetRequiredService<IMasaStackConfig>().GetServiceId(MasaStackConstant.AUTH)));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
