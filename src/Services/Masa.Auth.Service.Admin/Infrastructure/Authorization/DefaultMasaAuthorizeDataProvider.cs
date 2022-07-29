// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultMasaAuthorizeDataProvider : IMasaAuthorizeDataProvider
{
    public string GetAccount()
    {
        return "admin";
    }

    public List<string> GetAllowCodeList()
    {
        return new List<string>
        {
            "*123333"
        };
    }
}
