// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public class KeyValue
{
    public string Key { get; set; } = "";

    public string Value { get; set; } = "";

    public override bool Equals(object? obj)
    {
        return obj is KeyValue keyValue && keyValue.Key == Key;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}
