// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Models;

public class UserAutoCompleteOptions : ConfigurationApiMasaConfigurationOptions
{
    [JsonIgnore]
    public override string AppId => "public-$Config";

    [JsonIgnore]
    public override string? ObjectName { get; } = "$public.ES.UserAutoComplete";

    public string Name { get; set; } = "";

    public string[] Nodes { get; set; } = { };

    public string Alias { get; set; } = "";

    public string Index { get; set; } = "";
}
