// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultMasaAuthorizeDataProvider : IMasaAuthorizeDataProvider
{
    readonly IUserContext _userContext;

    public DefaultMasaAuthorizeDataProvider(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public string GetAccount()
    {
        return _userContext.UserName ?? "";
    }

    public List<string> GetAllowCodeList()
    {
        return new List<string>
        {
            "*123333"
        };
    }
}
