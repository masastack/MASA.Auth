// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class IdentityResourceModel
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<string> UserClaims { get; set; }

    public IdentityResourceModel(string name, string description, List<string> userClaims)
    {
        Name = name;
        Description = description;
        UserClaims = userClaims;
    }
}

