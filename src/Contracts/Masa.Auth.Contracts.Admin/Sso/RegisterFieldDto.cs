// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class RegisterFieldDto
{
    public RegisterFieldTypes RegisterFieldType { get; set; }

    public int Sort { get; set; }

    public bool Required { get; set; }

    public bool CannotUpdate { get; private set; }

    [JsonConstructor]
    public RegisterFieldDto(RegisterFieldTypes registerFieldType, int sort, bool required)
    {
        RegisterFieldType = registerFieldType;
        Sort = sort;
        Required = required;
        if(RegisterFieldType is RegisterFieldTypes.Account or RegisterFieldTypes.Password)
        {
            CannotUpdate = true;
        }     
    }
}

