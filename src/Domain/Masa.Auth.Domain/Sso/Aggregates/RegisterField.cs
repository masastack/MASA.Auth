// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Sso.Aggregates;

public class RegisterField : Entity<int>
{
    public RegisterFieldTypes RegisterFieldType { get; private set; }

    public int CustomLoginId { get; private set; }

    public int Sort { get; private set; }

    public bool Required { get; private set; }

    public RegisterField(RegisterFieldTypes registerFieldType, int sort, bool required)
    {
        RegisterFieldType = registerFieldType;
        Sort = sort;
        Required = required;
    }

    public RegisterField(int id, RegisterFieldTypes registerFieldType, int sort, bool required)
    {
        Id = id;
        RegisterFieldType = registerFieldType;
        Sort = sort;
        Required = required;
    }
}

