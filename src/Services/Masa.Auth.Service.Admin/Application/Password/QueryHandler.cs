// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Password;

public class QueryHandler
{
    private readonly IPasswordRuleProvider _passwordRuleProvider;

    public QueryHandler(IPasswordRuleProvider passwordRuleProvider)
    {
        _passwordRuleProvider = passwordRuleProvider;
    }

    [EventHandler]
    public void GlobalPasswordRuleQueryAsync(GlobalPasswordRuleQuery query)
    {
        query.Result = _passwordRuleProvider.GetGlobalPasswordRule();
    }
}
