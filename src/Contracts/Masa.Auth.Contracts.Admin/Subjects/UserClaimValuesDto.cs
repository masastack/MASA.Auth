// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin;

public class UserClaimValuesDto
{
    public Guid UserId { get; set; }

    public List<ClaimValue> ClaimValues { get; set; } = new();

    public class ClaimValue
    {
        public ClaimValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
