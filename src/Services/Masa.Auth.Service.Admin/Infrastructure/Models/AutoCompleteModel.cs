﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class AutoCompleteModel
{
    public string Name { get; set; } = string.Empty;

    public List<string> Nodes { get; set; } = new();

    public List<DocumentModel> Documents { get; set; } = new();
}