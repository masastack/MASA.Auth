// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultRuleCodePolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
    public IMasaStackConfig _masaStackConfig;

    public DefaultRuleCodePolicyProvider(IOptions<AuthorizationOptions> options, IMasaStackConfig masaStackConfig)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        _masaStackConfig = masaStackConfig;
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
            policy.AddRequirements(new DefaultRuleCodeRequirement(_masaStackConfig.GetServerId("auth")));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
