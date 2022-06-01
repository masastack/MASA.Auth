// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class AvatarValue : ValueObject
{
    public string Url { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Color { get; private set; } = string.Empty;

    public AvatarValue(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public AvatarValue(string name, string color, string url) : this(name, color)
    {
        Url = url;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Url;
        yield return Name;
        yield return Color;
    }
}
